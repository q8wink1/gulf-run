using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Coordinates currencies, inventory value, and monetization-adjacent rules
    /// on the client, always deferring authoritative validation to the backend.
    /// References: P012 (Economy System), P045 (Monetization System).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EconomyManager : Singleton<EconomyManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement currency/inventory client-side cache
            // once backend economy endpoints are available.
        }
    }
}
