using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Character.Configuration
{
    /// <summary>
    /// The single source of truth for "which characters exist" — 12 at
    /// launch, all unlocked from the start (Sprint 8 brief), with unlimited
    /// room to grow. Mirrors the role <c>WeaponCatalogConfig</c>/
    /// <c>TrapCatalogConfig</c> play for their sprints; the only difference
    /// is <see cref="Domain.CharacterId"/> is a free-form string, not an
    /// enum, so growing this list is purely a data change (see that type's
    /// remarks).
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterCatalogConfig", menuName = "GulfRun/Character/Character Catalog Config")]
    public sealed class CharacterCatalogConfig : ScriptableObject
    {
        [SerializeField] private List<CharacterDefinition> characters = new List<CharacterDefinition>();

        private readonly Dictionary<CharacterId, CharacterDefinition> _byId = new Dictionary<CharacterId, CharacterDefinition>();
        private bool _indexed;

        public IReadOnlyList<CharacterDefinition> Characters => characters;

        /// <summary>All characters are unlocked from the beginning (Sprint 8 brief) — the first catalog entry is simply the initial selection for a brand-new account.</summary>
        public CharacterId DefaultCharacterId => characters.Count > 0 && characters[0] != null ? characters[0].Id : CharacterId.None;

        public CharacterDefinition GetDefinition(CharacterId id)
        {
            EnsureIndexed();
            return _byId.TryGetValue(id, out CharacterDefinition definition) ? definition : null;
        }

        private void EnsureIndexed()
        {
            if (_indexed)
            {
                return;
            }

            _byId.Clear();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null)
                {
                    _byId[characters[i].Id] = characters[i];
                }
            }

            _indexed = true;
        }

#if UNITY_EDITOR
        private void OnValidate() => _indexed = false;
#endif
    }
}
