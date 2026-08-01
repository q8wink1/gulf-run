using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Configuration
{
    /// <summary>
    /// Per-country presentation data for the Victory Ceremony's national
    /// flags (Sprint 7 addendum) — one entry per <see cref="GulfCountry"/>,
    /// the same "one ScriptableObject catalog, no hardcoded per-item data in
    /// code" convention as <c>WeaponCatalogConfig</c>/<c>TrapCatalogConfig</c>.
    /// <see cref="FlagEntry.FlagSprite"/> is left unassigned today (no final
    /// art — see the Sprint 7 addendum report); until it is, presentation
    /// code falls back to <see cref="FlagEntry.PlaceholderColor"/> +
    /// <see cref="FlagEntry.Code"/> (a 3-letter code, e.g. "KSA"), the same
    /// "real system, honest placeholder visual" approach every prior
    /// sprint's catalogs use (e.g. <c>Trap.debugTint</c>).
    /// </summary>
    [CreateAssetMenu(fileName = "FlagCatalogConfig", menuName = "GulfRun/RaceFinish/Flag Catalog Config")]
    public sealed class FlagCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class FlagEntry
        {
            [SerializeField] private GulfCountry country;
            [SerializeField] private string code = "???";
            [SerializeField] private Sprite flagSprite;
            [SerializeField] private Color placeholderColor = Color.white;

            public GulfCountry Country => country;
            public string Code => code;
            public Sprite FlagSprite => flagSprite;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<FlagEntry> flags = new List<FlagEntry>();

        public bool TryGetFlag(GulfCountry country, out FlagEntry entry)
        {
            for (int i = 0; i < flags.Count; i++)
            {
                if (flags[i].Country == country)
                {
                    entry = flags[i];
                    return true;
                }
            }

            entry = null;
            return false;
        }
    }
}
