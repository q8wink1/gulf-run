using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Facade for all backend service calls (authentication, profile, inventory,
    /// currencies, matchmaking, and related systems). The backend remains the
    /// single source of truth; this manager never makes authoritative gameplay
    /// decisions on the client.
    /// References: P039 (Backend Architecture), P040 (Database Architecture),
    /// P041 (Authentication System).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BackendManager : Singleton<BackendManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 3+): Implement backend API client once the provider,
            // hosting and protocol decisions (P039 "Not Defined") are made.
        }
    }
}
