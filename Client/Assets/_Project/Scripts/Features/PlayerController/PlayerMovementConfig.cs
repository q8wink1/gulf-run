using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Designer-exposed, hot-swappable tuning values for player movement.
    /// No movement value may be hardcoded in behaviour scripts; everything
    /// gameplay-relevant is read from an instance of this asset.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PlayerMovementConfig",
        menuName = "GulfRun/Player/Movement Config")]
    public sealed class PlayerMovementConfig : ScriptableObject
    {
        [Header("Auto Run")]
        [SerializeField] private float autoRunSpeed = 5f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float doubleJumpForce = 6f;
        [SerializeField] private int maxJumpCount = 2;

        [Header("Ground Detection")]
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask groundLayerMask;

        [Header("Physics")]
        [SerializeField] private float gravityScale = 3f;

        public float AutoRunSpeed => autoRunSpeed;
        public float JumpForce => jumpForce;
        public float DoubleJumpForce => doubleJumpForce;
        public int MaxJumpCount => maxJumpCount;
        public float GroundCheckRadius => groundCheckRadius;
        public LayerMask GroundLayerMask => groundLayerMask;
        public float GravityScale => gravityScale;
    }
}
