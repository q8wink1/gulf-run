using System.Collections.Generic;
using GulfRun.Core.Pooling;
using GulfRun.Domain;
using GulfRun.Features.Traps.Configuration;
using GulfRun.Features.Traps.Spawning;
using UnityEngine;

namespace GulfRun.Features.Traps
{
    /// <summary>
    /// Debug overlay: Current Trap Count, Spawn Positions, Lifetime Timer,
    /// Pool Usage, Trap IDs — reads exclusively from
    /// <see cref="TrapSpawnController"/> (populated on every client from the
    /// host's broadcasts) rather than <c>TrapAuthority</c>, since the
    /// latter's active-trap bookkeeping only exists host-side. Same
    /// OnGUI-placeholder approach as RunnerDebugView/MultiplayerDebugView/
    /// WeaponsDebugView; placed further right again so all four panels can
    /// be shown at once.
    /// </summary>
    public sealed class TrapsDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 1360;
        [SerializeField] private TrapCatalogConfig catalog;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = 10;
            const int lineHeight = 18;
            const int width = 460;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            TrapSpawnController controller = TrapSpawnController.Instance;
            int activeCount = controller != null ? controller.ActiveTraps.Count : 0;

            Line($"[Traps] Catalog: {(catalog != null ? catalog.Traps.Count : 0)} trap types");
            Line($"Current Trap Count: {activeCount}");

            if (controller != null)
            {
                double now = Time.timeAsDouble;
                foreach (KeyValuePair<int, ActiveTrapView> entry in controller.ActiveTraps)
                {
                    Vector3 pos = entry.Value.Instance != null ? entry.Value.Instance.transform.position : Vector3.zero;
                    float remaining = Mathf.Max(0f, (float)(entry.Value.ExpireAtSeconds - now));
                    Line($"  #{entry.Key} [{entry.Value.Trap}] pos=({pos.x:F1}, {pos.y:F1}) life={remaining:F1}s");
                }
            }

            y += 6;
            Line("Pool Usage:");
            if (ObjectPoolManager.Instance != null)
            {
                foreach (PoolStats stats in ObjectPoolManager.Instance.GetAllStats())
                {
                    if (stats.PoolName.Contains("Trap"))
                    {
                        Line($"  {stats.PoolName}: active={stats.Active} inactive={stats.Inactive}");
                    }
                }
            }

            y += 6;
            Line("Trap IDs / Spawn Weights:");
            if (catalog != null)
            {
                float totalWeight = 0f;
                foreach (WeightedOption<TrapId> option in catalog.GetWeightedOptions())
                {
                    totalWeight += option.Weight;
                }

                foreach (WeightedOption<TrapId> option in catalog.GetWeightedOptions())
                {
                    float rate = totalWeight > 0f ? option.Weight / totalWeight * 100f : 0f;
                    Line($"  [{option.Value}] {rate:F1}%");
                }
            }
        }
#endif
    }
}
