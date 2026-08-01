namespace GulfRun.Core.Pooling
{
    /// <summary>
    /// Optional lifecycle hook for components on pooled GameObjects. Implement
    /// this to reset per-spawn state (e.g. re-arm a trigger, clear a timer)
    /// without the pool needing to know anything about the specific object.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>Called immediately after the object is activated and positioned.</summary>
        void OnSpawned();

        /// <summary>Called immediately before the object is deactivated and returned to the pool.</summary>
        void OnDespawned();
    }
}
