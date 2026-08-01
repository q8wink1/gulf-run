using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Abstraction over "apply a weapon's gameplay effect to this player",
    /// implemented by <c>Features.PlayerController.PlayerStatusEffectController</c>.
    /// Exists in Core (like <see cref="ILocalPlayerStateProvider"/> and
    /// <see cref="IGameStateProvider"/>) so Features.Weapons can apply
    /// confirmed hits without referencing the PlayerController feature
    /// directly — see <see cref="PlayerStatusEffectRegistry"/>.
    /// </summary>
    public interface IPlayerStatusEffectReceiver
    {
        /// <summary>True while a Falcon Feather Mark is active on this player.</summary>
        bool IsMarked { get; }

        /// <summary>
        /// Attempts to apply <paramref name="effect"/>. Returns false if it
        /// was fully blocked (an active Protection Shield absorbed it,
        /// consuming the shield) — callers use this to decide whether to
        /// still play "blocked" feedback instead of "applied" feedback.
        /// </summary>
        bool TryApplyEffect(PlayerStatusEffect effect);

        /// <summary>Clears an active Falcon Feather Mark — called once this player's confirmed hit against a marked opponent has been resolved.</summary>
        void ClearMark();
    }
}
