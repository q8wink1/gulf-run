namespace GulfRun.Domain
{
    /// <summary>
    /// Time of day chosen once per race, purely for lighting (Sprint 12
    /// brief: "TIME OF DAY: Randomly choose before every race ... Lighting
    /// changes only. Gameplay remains identical."). Never read by any
    /// gameplay/collision code — only by Features.Maps' lighting
    /// application, ambient audio selection, and debug/UI.
    /// </summary>
    public enum TimeOfDay
    {
        Morning,
        Sunset,
        Night
    }
}
