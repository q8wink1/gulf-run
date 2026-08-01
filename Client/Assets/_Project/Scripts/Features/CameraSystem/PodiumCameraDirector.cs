using GulfRun.Core.Networking;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.CameraSystem
{
    /// <summary>
    /// "Camera movement" for the Victory Ceremony (Sprint 7): pans the main
    /// camera to a fixed podium framing while the Podium phase is active,
    /// then hands control straight back to <see cref="SideScrollCameraFollow"/>
    /// once the phase ends. Reacts to <see cref="RaceEndPhase"/> purely via
    /// <see cref="IMatchTransport"/> (Core/Domain only) — never references
    /// Features.RaceFinish directly, since Features must not reference other
    /// Features (see docs/02-architecture/FOLDER_ARCHITECTURE.md §4).
    /// </summary>
    public sealed class PodiumCameraDirector : MonoBehaviour
    {
        [SerializeField] private SideScrollCameraFollow follow;
        [SerializeField] private Vector3 podiumCameraPosition = new Vector3(0f, 2f, -8f);
        [SerializeField] private float panSmoothTime = 0.4f;

        private Vector3 _velocity;
        private bool _podiumActive;

        private void OnEnable()
        {
            MatchTransportService.Current.RaceEndPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null)
            {
                transport.RaceEndPhaseChanged -= HandlePhaseChanged;
            }
        }

        private void LateUpdate()
        {
            if (!_podiumActive)
            {
                return;
            }

            transform.position = Vector3.SmoothDamp(transform.position, podiumCameraPosition, ref _velocity, panSmoothTime);
        }

        private void HandlePhaseChanged(RaceEndPhase phase)
        {
            _podiumActive = phase == RaceEndPhase.Podium;

            if (follow != null)
            {
                follow.enabled = !_podiumActive;
            }
        }
    }
}
