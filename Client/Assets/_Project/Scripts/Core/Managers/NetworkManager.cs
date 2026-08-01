using GulfRun.Core.Networking;
using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Client-side entry point for network communication with the backend and,
    /// as of Sprint 4, real-time multiplayer transport bootstrap. Named "NetworkManager"
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
            // TODO(Sprint 5+): Backend meta connection (auth, inventory, etc.)
            // is still unimplemented.
            //
            // Sprint 4: make sure a match transport exists as early as
            // possible in the boot sequence. MatchTransportService.Current
            // already self-initializes with a LocalLoopbackTransport on
            // first access, so this call is redundant defense-in-depth today
            // — but it is the designated place to assign a *real* transport
            // (Unity Netcode for GameObjects, a custom protocol, or a third
            // party) once docs/adr/0001-multiplayer-transport-abstraction.md
            // is ratified, instead of relying on the offline-testing default.
            if (MatchTransportService.Current == null)
            {
                MatchTransportService.Current = new LocalLoopbackTransport();
            }
        }
    }
}
