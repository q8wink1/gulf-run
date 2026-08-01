using UnityEngine;

namespace GulfRun.Features.EndlessRunner.Configuration
{
    /// <summary>Tuning values for the global Game Speed Controller.</summary>
    [CreateAssetMenu(
        fileName = "GameSpeedConfig",
        menuName = "GulfRun/EndlessRunner/Game Speed Config")]
    public sealed class GameSpeedConfig : ScriptableObject
    {
        [Header("Progressive Speed")]
        [SerializeField] private float baseSpeed = 5f;
        [SerializeField] private float maxSpeed = 12f;
        [SerializeField] private float rampDistanceMeters = 1500f;

        public float BaseSpeed => baseSpeed;
        public float MaxSpeed => maxSpeed;
        public float RampDistanceMeters => rampDistanceMeters;
    }
}
