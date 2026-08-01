using System.Collections.Generic;
using UnityEngine;

namespace GulfRun.Core.Pooling
{
    /// <summary>Snapshot of a single pool's usage, for debug tooling.</summary>
    public readonly struct PoolStats
    {
        public readonly string PoolName;
        public readonly int Active;
        public readonly int Inactive;

        public PoolStats(string poolName, int active, int inactive)
        {
            PoolName = poolName;
            Active = active;
            Inactive = inactive;
        }
    }

    /// <summary>
    /// Generic, prefab-keyed object pool manager shared by every gameplay
    /// system (world chunks, obstacles, coins, power-ups, decorations, and
    /// any future spawnable). Gameplay code must never Instantiate/Destroy
    /// directly — only Preload/Get/Release through this manager — so pooled
    /// objects are always reused instead of allocated during play.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private readonly Dictionary<GameObject, GameObjectPool> _pools = new Dictionary<GameObject, GameObjectPool>();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new Dictionary<GameObject, GameObject>();

        protected override void OnInitialize()
        {
        }

        /// <summary>Warms up a prefab's pool ahead of time so gameplay never pays an Instantiate cost.</summary>
        public void Preload(GameObject prefab, int count, Transform parent = null)
        {
            if (prefab == null || count <= 0)
            {
                return;
            }

            GetOrCreatePool(prefab, parent).Preload(count);
        }

        /// <summary>Activates and positions a pooled instance of <paramref name="prefab"/>, expanding the pool if needed.</summary>
        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                return null;
            }

            GameObjectPool pool = GetOrCreatePool(prefab, parent);
            GameObject instance = pool.Get(position, rotation, parent);
            _instanceToPrefab[instance] = prefab;
            return instance;
        }

        /// <summary>Deactivates <paramref name="instance"/> and returns it to its source pool. Returns false if it was not a pooled instance (or already released).</summary>
        public bool Release(GameObject instance)
        {
            if (instance == null)
            {
                return false;
            }

            if (!_instanceToPrefab.TryGetValue(instance, out GameObject prefab))
            {
                return false;
            }

            if (_pools.TryGetValue(prefab, out GameObjectPool pool) && pool.Release(instance))
            {
                _instanceToPrefab.Remove(instance);
                return true;
            }

            return false;
        }

        /// <summary>Per-prefab active/inactive counts, for the runner debug view.</summary>
        public IEnumerable<PoolStats> GetAllStats()
        {
            foreach (KeyValuePair<GameObject, GameObjectPool> entry in _pools)
            {
                string name = entry.Key != null ? entry.Key.name : "(destroyed prefab)";
                yield return new PoolStats(name, entry.Value.ActiveCount, entry.Value.InactiveCount);
            }
        }

        private GameObjectPool GetOrCreatePool(GameObject prefab, Transform parent)
        {
            if (!_pools.TryGetValue(prefab, out GameObjectPool pool))
            {
                Transform poolParent = parent != null ? parent : transform;
                pool = new GameObjectPool(prefab, poolParent);
                _pools[prefab] = pool;
            }

            return pool;
        }
    }
}
