using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only view of the local player's live movement state, published
    /// by the PlayerController feature (see
    /// <c>Features.PlayerController.PlayerNetworkStateAdapter</c>) and
    /// consumed by the Multiplayer feature for network sync — same
    /// decoupling pattern as <see cref="IRunSpeedProvider"/> and
    /// <see cref="IGameStateProvider"/>, so neither feature ever references
    /// the other's assembly.
    /// </summary>
    public interface ILocalPlayerStateProvider
    {
        Vector2 Position { get; }
        float RotationDegrees { get; }
        PlayerMovementState AnimationState { get; }
    }
}
