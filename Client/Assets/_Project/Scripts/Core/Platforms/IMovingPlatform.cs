using UnityEngine;

namespace GulfRun.Core.Platforms
{
    /// <summary>
    /// Optional capability for any grounded surface that moves under its own
    /// power (a moving platform, a future flying carpet / mount, etc.).
    /// <see cref="GulfRun.Features.PlayerController.PlayerGroundDetector"/> looks
    /// for this component on whatever collider it is currently standing on;
    /// when present, the motor carries the player along with the surface's
    /// per-frame movement instead of leaving them behind. Lives in Core (not
    /// a Feature) so both PlayerController and any future world-generation
    /// platform prefab can depend on it without the two features referencing
    /// each other.
    /// </summary>
    public interface IMovingPlatform
    {
        /// <summary>World-space movement applied by this platform during the current frame.</summary>
        Vector2 FrameDelta { get; }
    }
}
