using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Coordinates local and cloud save/load of player data. Device-specific
    /// settings may remain local; account-linked data synchronizes with the
    /// backend, which remains the source of truth.
    /// References: P034 (Settings System), P039 (Backend Architecture),
    /// P040 (Database Architecture).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SaveManager : Singleton<SaveManager>
    {
        protected override void OnInitialize()
        {
            // TODO(Sprint 2+): Implement local persistence and cloud save
            // synchronization once the Backend/Database systems are online.
        }
    }
}
