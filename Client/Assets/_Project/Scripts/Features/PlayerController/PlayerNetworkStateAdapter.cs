using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Publishes this local player's live position/rotation/animation state
    /// through <see cref="ILocalPlayerStateProvider"/>/
    /// <see cref="LocalPlayerStateService"/> so the Multiplayer feature can
    /// read it for network sync without PlayerController ever referencing
    /// that feature directly — the exact decoupling pattern already used for
    /// <see cref="RunSpeedService"/>/<see cref="GameStateService"/>.
    ///
    /// Optional: add this component alongside <see cref="PlayerMotor"/> only
    /// on a Player instance that is participating in a networked match; a
    /// stand-alone/single-player Player.prefab does not need it.
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    public sealed class PlayerNetworkStateAdapter : MonoBehaviour, ILocalPlayerStateProvider
    {
        private PlayerMotor _motor;

        public Vector2 Position => transform.position;

        // The player never visually rotates in the current side-scroller
        // (Sprint 2/3 movement is upright-only); reserved for a future
        // lean/tilt, vehicle, or Flying Carpet mechanic.
        public float RotationDegrees => 0f;

        public PlayerMovementState AnimationState => _motor.CurrentState;

        private void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
        }

        private void OnEnable()
        {
            LocalPlayerStateService.Current = this;
        }

        private void OnDisable()
        {
            if (ReferenceEquals(LocalPlayerStateService.Current, this))
            {
                LocalPlayerStateService.Current = null;
            }
        }
    }
}
