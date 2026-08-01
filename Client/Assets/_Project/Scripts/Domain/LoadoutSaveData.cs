using System;
using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure snapshot of the local player's Locker selections + ownership for
    /// persistence (Sprint 16). No Unity dependency — serialized to a compact
    /// string by <c>Core.Managers.SaveManager</c>.
    /// </summary>
    public sealed class LoadoutSaveData
    {
        public CharacterId CharacterId { get; set; } = CharacterId.None;
        public Dictionary<CosmeticSlot, CosmeticId> Equipped { get; } = new Dictionary<CosmeticSlot, CosmeticId>();
        public List<string> PermanentOwnedIds { get; } = new List<string>();
        public List<TemporaryCosmeticOwnership> TemporaryOwned { get; } = new List<TemporaryCosmeticOwnership>();

        /// <summary>Compact pipe/semicolon encoding safe for PlayerPrefs strings.</summary>
        public string Encode()
        {
            var parts = new List<string>
            {
                "v1",
                CharacterId.Value ?? string.Empty
            };

            var equippedParts = new List<string>();
            foreach (KeyValuePair<CosmeticSlot, CosmeticId> pair in Equipped)
            {
                if (!pair.Value.IsNone)
                {
                    equippedParts.Add(((int)pair.Key) + "=" + pair.Value.Value);
                }
            }

            parts.Add(string.Join(",", equippedParts));
            parts.Add(string.Join(",", PermanentOwnedIds));

            var temporaryParts = new List<string>();
            for (int i = 0; i < TemporaryOwned.Count; i++)
            {
                TemporaryCosmeticOwnership entry = TemporaryOwned[i];
                if (!entry.Id.IsNone)
                {
                    temporaryParts.Add(entry.Id.Value + "=" + entry.ExpiresAtSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            parts.Add(string.Join(",", temporaryParts));
            return string.Join("|", parts);
        }

        public static bool TryDecode(string raw, out LoadoutSaveData data)
        {
            data = null;
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            string[] parts = raw.Split('|');
            if (parts.Length < 5 || parts[0] != "v1")
            {
                return false;
            }

            var result = new LoadoutSaveData
            {
                CharacterId = new CharacterId(parts[1])
            };

            if (!string.IsNullOrEmpty(parts[2]))
            {
                string[] equipped = parts[2].Split(',');
                for (int i = 0; i < equipped.Length; i++)
                {
                    string[] kv = equipped[i].Split('=');
                    if (kv.Length == 2 && int.TryParse(kv[0], out int slotInt) && Enum.IsDefined(typeof(CosmeticSlot), slotInt))
                    {
                        result.Equipped[(CosmeticSlot)slotInt] = new CosmeticId(kv[1]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(parts[3]))
            {
                string[] owned = parts[3].Split(',');
                for (int i = 0; i < owned.Length; i++)
                {
                    if (!string.IsNullOrEmpty(owned[i]))
                    {
                        result.PermanentOwnedIds.Add(owned[i]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(parts[4]))
            {
                string[] temporary = parts[4].Split(',');
                for (int i = 0; i < temporary.Length; i++)
                {
                    string[] kv = temporary[i].Split('=');
                    if (kv.Length == 2 &&
                        double.TryParse(kv[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double expires))
                    {
                        result.TemporaryOwned.Add(new TemporaryCosmeticOwnership(new CosmeticId(kv[0]), 0d, expires));
                    }
                }
            }

            data = result;
            return true;
        }
    }
}
