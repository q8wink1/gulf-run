using UnityEngine;

namespace GulfRun.Core.Pooling
{
    /// <summary>
    /// Marker/back-reference component automatically attached to every
    /// instance created by <see cref="ObjectPoolManager"/>. Lets callers
    /// release an instance with just <c>Release(instance)</c> instead of
    /// having to remember which prefab it came from.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PooledObjectHandle : MonoBehaviour
    {
        public GameObject SourcePrefab { get; internal set; }
    }
}
