using UnityEngine;

namespace GulfRun.Features.CameraSystem
{
    /// <summary>
    /// Sprint 23.5 — empty stubs for future camera polish.
    /// Speed FOV increase, motion blur, and cinematic transitions are not implemented yet.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CameraEffectsPlaceholder : MonoBehaviour
    {
        [Header("Placeholders — inactive / unused")]
        [SerializeField] private bool enableSpeedFovIncrease;
        [SerializeField] private bool enableMotionBlur;
        [SerializeField] private bool enableCinematicTransitions;

        // Intentionally empty — wire real effects in a later sprint.
        private void OnEnable()
        {
            _ = enableSpeedFovIncrease;
            _ = enableMotionBlur;
            _ = enableCinematicTransitions;
        }
    }
}
