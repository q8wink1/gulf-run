namespace GulfRun.Domain
{
    /// <summary>
    /// Pure ordinal formatting for live race place ("1st", "2nd", …) — no
    /// UnityEngine dependency, same engine-free Domain posture as
    /// <see cref="CelebrationAnimation"/>.
    /// </summary>
    public static class RacePositionFormatter
    {
        public static string FormatOrdinal(int place)
        {
            if (place <= 0)
            {
                return "—";
            }

            int mod100 = place % 100;
            if (mod100 >= 11 && mod100 <= 13)
            {
                return place + "th";
            }

            return (place % 10) switch
            {
                1 => place + "st",
                2 => place + "nd",
                3 => place + "rd",
                _ => place + "th"
            };
        }
    }
}
