using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Character.Configuration
{
    /// <summary>
    /// Data-driven definition of exactly one playable character. Per P005
    /// CHR-005 ("identical gameplay statistics; no gameplay advantages"),
    /// this deliberately carries zero gameplay-affecting fields — only
    /// identity and presentation. Adding character #13 is authoring a new
    /// asset and adding it to <see cref="CharacterCatalogConfig"/>, never a
    /// code change (see <see cref="Domain.CharacterId"/>'s remarks).
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterDefinition", menuName = "GulfRun/Character/Character Definition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private CharacterGenderPresentation genderPresentation;

        [Header("Presentation (final art TODO — same 'no final art yet' status as prior sprints)")]
        [SerializeField] private GameObject previewPrefab;
        [SerializeField] private Sprite portraitIcon;
        [SerializeField] private Color placeholderColor = Color.white;

        public CharacterId Id => new CharacterId(id);
        public string DisplayName => string.IsNullOrEmpty(displayName) ? id : displayName;
        public CharacterGenderPresentation GenderPresentation => genderPresentation;
        public GameObject PreviewPrefab => previewPrefab;
        public Sprite PortraitIcon => portraitIcon;
        public Color PlaceholderColor => placeholderColor;
    }
}
