using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only + claim seam onto the local player's Login Streak state
    /// (Sprint 13 "LOGIN REWARD" popup). Implemented by
    /// <c>Features.Progression.Login.LoginRewardManager</c> so
    /// Features.MainMenu can show/claim it without ever referencing
    /// Features.Progression — the same shape as <see cref="IDailyMissionsPreviewProvider"/>.
    /// </summary>
    public interface ILoginRewardStatusProvider
    {
        LoginStreakStatus Status { get; }

        string ActiveSpecialEventLabel { get; }

        bool HasClaimedToday();

        bool TryClaimDailyLogin();
    }
}
