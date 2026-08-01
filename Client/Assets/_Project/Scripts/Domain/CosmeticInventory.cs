using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// One player's owned cosmetic set. Pure data/logic — no Unity
    /// dependency, matching <see cref="WeaponInventory"/>'s style — so the
    /// same ownership rules (COS-OWN-001: "Unlocked cosmetics become
    /// permanently owned") can run identically once a real backend/database
    /// (P039/P040) is wired in.
    /// <para>
    /// Sprint 11 note: also tracks TEMPORARY ownership (Daily Mission /
    /// Login Reward grants with an expiry — brief "TEMPORARY COSMETICS")
    /// in a separate map, never mixed into the permanent set. A permanent
    /// grant always wins and clears any temporary entry for the same id
    /// (COS-OWN-001 still holds: once permanently owned, always owned) —
    /// this is the "Store lets a temporary owner pay to Unlock Permanently"
    /// upgrade path. A temporary grant is refused outright if the id is
    /// already permanently owned (brief: "If player already owns permanent
    /// version: Never reward temporary duplicate"), which the reward-
    /// granting caller (<c>Features.Progression</c>) is expected to check
    /// via <see cref="OwnsPermanently"/> before choosing an alternative
    /// reward, but is enforced here too as a safety net.
    /// </para>
    /// </summary>
    public sealed class CosmeticInventory
    {
        private readonly HashSet<string> _owned = new HashSet<string>();
        private readonly Dictionary<string, double> _temporaryExpiresAtSeconds = new Dictionary<string, double>();

        public bool OwnsPermanently(CosmeticId cosmetic) => !cosmetic.IsNone && _owned.Contains(cosmetic.Value);

        public bool OwnsTemporarily(CosmeticId cosmetic) => !cosmetic.IsNone && _temporaryExpiresAtSeconds.ContainsKey(cosmetic.Value);

        /// <summary>True whether ownership is permanent or a still-active temporary grant.</summary>
        public bool Owns(CosmeticId cosmetic) => OwnsPermanently(cosmetic) || OwnsTemporarily(cosmetic);

        /// <summary>Grants permanent ownership. Idempotent. Also clears any temporary grant for the same id — permanent always supersedes temporary.</summary>
        public void Grant(CosmeticId cosmetic)
        {
            if (cosmetic.IsNone)
            {
                return;
            }

            _owned.Add(cosmetic.Value);
            _temporaryExpiresAtSeconds.Remove(cosmetic.Value);
        }

        /// <summary>Grants temporary ownership until <paramref name="expiresAtSeconds"/> (real-world/Unix epoch seconds). Refuses (returns false) for <see cref="CosmeticId.None"/> or an already-permanently-owned id — never downgrades or duplicates.</summary>
        public bool GrantTemporary(CosmeticId cosmetic, double expiresAtSeconds)
        {
            if (cosmetic.IsNone || OwnsPermanently(cosmetic))
            {
                return false;
            }

            _temporaryExpiresAtSeconds[cosmetic.Value] = expiresAtSeconds;
            return true;
        }

        public bool TryGetTemporaryExpiry(CosmeticId cosmetic, out double expiresAtSeconds) => _temporaryExpiresAtSeconds.TryGetValue(cosmetic.Value, out expiresAtSeconds);

        /// <summary>Removes every temporary grant whose expiry is at-or-before <paramref name="nowSeconds"/> (brief: "When expired: Item is automatically removed"). Returns the removed ids so the caller can also unequip them from any <see cref="CosmeticSlot"/> — never allocates when nothing expired.</summary>
        public List<string> RemoveExpired(double nowSeconds)
        {
            List<string> expired = null;
            foreach (KeyValuePair<string, double> entry in _temporaryExpiresAtSeconds)
            {
                if (entry.Value <= nowSeconds)
                {
                    (expired ??= new List<string>()).Add(entry.Key);
                }
            }

            if (expired != null)
            {
                for (int i = 0; i < expired.Count; i++)
                {
                    _temporaryExpiresAtSeconds.Remove(expired[i]);
                }
            }

            return expired ?? EmptyStringList;
        }

        private static readonly List<string> EmptyStringList = new List<string>();

        /// <summary>Permanently-owned ids only.</summary>
        public IReadOnlyCollection<string> OwnedIds => _owned;

        /// <summary>Currently-active (unexpired) temporarily-owned ids only.</summary>
        public IReadOnlyCollection<string> TemporaryOwnedIds => _temporaryExpiresAtSeconds.Keys;
    }
}
