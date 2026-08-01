using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>Pure "are we ready to start the countdown" rule for the Ready System.</summary>
    public static class ReadyCheck
    {
        public static bool AllReady(IReadOnlyList<PlayerReadyState> readyStates, int minimumPlayers)
        {
            if (readyStates == null || readyStates.Count < minimumPlayers)
            {
                return false;
            }

            for (int i = 0; i < readyStates.Count; i++)
            {
                if (readyStates[i] != PlayerReadyState.Ready)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
