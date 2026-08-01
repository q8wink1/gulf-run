using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Core.Countries
{
    /// <summary>
    /// The single source of truth for per-country presentation data — code,
    /// display name, and flag (sprite or placeholder color) — for the eight
    /// Sprint 8 launch nations. Deliberately lives in <c>GulfRun.Core</c>
    /// (not any one Feature) because Country now determines National Flag,
    /// Profile Flag, Lobby Flag, AND Podium Flag (Sprint 8 brief), so both
    /// <c>Features.Character</c> (Account Creation, Character Menu) and
    /// <c>Features.Multiplayer</c> (a future Lobby screen) need the exact
    /// same data without depending on each other.
    ///
    /// Sprint 7's <c>Features.RaceFinish.Configuration.FlagCatalogConfig</c>
    /// (the Podium Ceremony's flag data) is intentionally left as-is rather
    /// than migrated onto this new catalog — that tested Sprint 7 code is
    /// out of scope for this sprint's changes (same "do not restart/break a
    /// prior sprint" posture the Sprint 7 addendum itself followed).
    /// Consolidating the two catalogs is a natural, low-risk follow-up
    /// (tracked in the Sprint 8 report's Remaining TODOs).
    /// </summary>
    [CreateAssetMenu(fileName = "CountryCatalogConfig", menuName = "GulfRun/Character/Country Catalog Config")]
    public sealed class CountryCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class CountryEntry
        {
            [SerializeField] private GulfCountry country;
            [SerializeField] private string code = "???";
            [SerializeField] private string displayName = string.Empty;
            [SerializeField] private Sprite flagSprite;
            [SerializeField] private Color placeholderColor = Color.white;

            public GulfCountry Country => country;
            public string Code => code;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? country.ToString() : displayName;
            public Sprite FlagSprite => flagSprite;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<CountryEntry> countries = new List<CountryEntry>();

        public IReadOnlyList<CountryEntry> Countries => countries;

        public bool TryGetEntry(GulfCountry country, out CountryEntry entry)
        {
            for (int i = 0; i < countries.Count; i++)
            {
                if (countries[i].Country == country)
                {
                    entry = countries[i];
                    return true;
                }
            }

            entry = null;
            return false;
        }
    }
}
