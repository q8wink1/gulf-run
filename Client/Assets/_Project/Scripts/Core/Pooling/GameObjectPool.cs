using System.Collections.Generic;
using UnityEngine;

namespace GulfRun.Core.Pooling
{
    /// <summary>
    /// A single prefab's pool of instances. Not exposed directly to gameplay
    /// code — accessed only through <see cref="ObjectPoolManager"/>, which
    /// owns one of these per distinct prefab.
    /// </summary>
    internal sealed class GameObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Stack<GameObject> _inactive = new Stack<GameObject>();
        private readonly HashSet<GameObject> _active = new HashSet<GameObject>();

        public int ActiveCount => _active.Count;
        public int InactiveCount => _inactive.Count;

        public GameObjectPool(GameObject prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject instance = CreateInstance();
                instance.SetActive(false);
                _inactive.Push(instance);
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject instance = _inactive.Count > 0 ? _inactive.Pop() : CreateInstance();

            Transform instanceTransform = instance.transform;
            instanceTransform.SetParent(parent != null ? parent : _parent, worldPositionStays: false);
            instanceTransform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            _active.Add(instance);

            foreach (IPoolable poolable in instance.GetComponents<IPoolable>())
            {
                poolable.OnSpawned();
            }

            return instance;
        }

        public bool Release(GameObject instance)
        {
            if (!_active.Remove(instance))
            {
                return false;
            }

            foreach (IPoolable poolable in instance.GetComponents<IPoolable>())
            {
                poolable.OnDespawned();
            }

            instance.SetActive(false);
            instance.transform.SetParent(_parent, worldPositionStays: false);
            _inactive.Push(instance);
            return true;
        }

        private GameObject CreateInstance()
        {
            GameObject instance = Object.Instantiate(_prefab, _parent);
            PooledObjectHandle handle = instance.GetComponent<PooledObjectHandle>();
            if (handle == null)
            {
                handle = instance.AddComponent<PooledObjectHandle>();
            }

            handle.SourcePrefab = _prefab;
            return instance;
        }
    }
}
