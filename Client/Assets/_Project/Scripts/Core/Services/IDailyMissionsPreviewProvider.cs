using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only + claim seam onto the local player's 3 active Daily
    /// Missions (Sprint 13 "DAILY MISSIONS" widget). Implemented by
    /// <c>Features.Progression.Missions.MissionManager</c> so
    /// Features.MainMenu can show/claim them without ever referencing
    /// Features.Progression — the same shape as <see cref="ILocalLoadoutProvider"/>.
    /// </summary>
    public interface IDailyMissionsPreviewProvider
    {
        IReadOnlyList<ActiveMission> ActiveMissions { get; }

        bool TryClaimMission(int slotIndex);
    }
}
