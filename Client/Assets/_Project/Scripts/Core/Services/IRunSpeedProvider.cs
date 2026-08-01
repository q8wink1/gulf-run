namespace GulfRun.Core.Services
{
    /// <summary>
    /// Abstraction over "what is the current global run speed right now".
    /// Exists in Core (not a Feature) so PlayerController and the future
    /// EndlessRunner speed system can both depend on this shared abstraction
    /// instead of referencing each other directly (features must stay
    /// decoupled from one another; see <see cref="RunSpeedService"/>).
    /// </summary>
    public interface IRunSpeedProvider
    {
        /// <summary>Current global run speed, in meters/second.</summary>
        float CurrentSpeed { get; }
    }
}
