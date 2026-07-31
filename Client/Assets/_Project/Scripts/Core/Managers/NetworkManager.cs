using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Client-side entry point for network communication with the backend and,
    /// in future sprints, real-time multiplayer transport. Named "NetworkManager"
    /// per the approved Sprint 1 brief; note this will collide with
    /// Unity.Netcode.NetworkManager once Netcode for GameObjects is introduced,
    /// so future code must fully qualify one of the two types.
    /// References: P039 (Backend Architecture, Data Synchronization).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NetworkManager : Singleton<NetworkManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement backend connection/session handling
            // once transport and Netcode preparation packages are configured.
        }
    }
}
