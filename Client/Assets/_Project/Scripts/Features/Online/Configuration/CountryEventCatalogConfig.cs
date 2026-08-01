using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Configuration
{
    /// <summary>
    /// Every "Country Event" from the Sprint 9 brief: one National Day per
    /// launch country, Ramadan/Eid Championships, Summer/Winter Events, and
    /// room for "future regional tournaments" — all as data rows tagged
    /// with a <see cref="CountryEventCategory"/>, never a new enum member
    /// or code change per event.
    /// </summary>
    [CreateAssetMenu(fileName = "CountryEventCatalogConfig", menuName = "GulfRun/Online/Country Event Catalog Config")]
    public sealed class CountryEventCatalogConfig : ScriptableObject
    {
        [Serializable]
        public sealed class CountryEventEntry
        {
            [SerializeField] private string id;
            [SerializeField] private CountryEventCategory category;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField, TextArea] private string description = string.Empty;

            [Tooltip("Only meaningful when isCountrySpecific is true (National Days) — which nation this event celebrates.")]
            [SerializeField] private bool isCountrySpecific;

            [SerializeField] private GulfCountry country;
            [SerializeField] private Color placeholderColor = Color.white;

            public string Id => id;
            public CountryEventCategory Category => category;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
            public string Description => description;
            public GulfCountry? Country => isCountrySpecific ? country : (GulfCountry?)null;
            public Color PlaceholderColor => placeholderColor;
        }

        [SerializeField] private List<CountryEventEntry> events = new List<CountryEventEntry>();

        public IReadOnlyList<CountryEventEntry> Events => events;
    }
}
