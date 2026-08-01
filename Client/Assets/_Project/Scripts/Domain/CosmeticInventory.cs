using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// One player's permanently-owned cosmetic set. Pure data/logic — no
    /// Unity dependency, matching <see cref="WeaponInventory"/>'s style —
    /// so the same ownership rules (COS-OWN-001: "Unlocked cosmetics become
    /// permanently owned") can run identically once a real backend/database
    /// (P039/P040) is wired in.
    /// </summary>
    public sealed class CosmeticInventory
    {
        private readonly HashSet<string> _owned = new HashSet<string>();

        public bool Owns(CosmeticId cosmetic) => !cosmetic.IsNone && _owned.Contains(cosmetic.Value);

        /// <summary>Grants permanent ownership. Idempotent — granting an already-owned cosmetic again is a no-op.</summary>
        public void Grant(CosmeticId cosmetic)
        {
            if (!cosmetic.IsNone)
            {
                _owned.Add(cosmetic.Value);
            }
        }

        public IReadOnlyCollection<string> OwnedIds => _owned;
    }
}
