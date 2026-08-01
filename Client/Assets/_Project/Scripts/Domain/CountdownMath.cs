using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure whole-seconds-remaining computation shared by any countdown
    /// timer. Kept separate from the single-player
    /// <c>Features.EndlessRunner.GameLoop.CountdownController</c> (which
    /// pairs this same math with MonoBehaviour/event plumbing) so the
    /// match-level countdown can reuse the arithmetic without either system
    /// depending on the other.
    /// </summary>
    public static class CountdownMath
    {
        public static int WholeSecondsRemaining(double elapsedSeconds, double durationSeconds)
        {
            double remaining = durationSeconds - elapsedSeconds;
            return remaining <= 0d ? 0 : (int)Math.Ceiling(remaining);
        }
    }
}
