using UnityEngine;

namespace GulfRun.Core
{
    /// <summary>
    /// Generic base class for MonoBehaviour-based manager singletons.
    /// Ensures a single persistent instance across scene loads.
    /// Derived managers are expected to be placed on a manager GameObject
    /// in the Boot scene and must not be duplicated in other scenes.
    /// </summary>
    /// <typeparam name="T">Concrete manager type.</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        /// <summary>
        /// Global access point for the manager instance.
        /// Returns null if the manager has not been initialized yet.
        /// </summary>
        public static T Instance => _instance;

        /// <summary>
        /// True once this singleton has completed <see cref="Awake"/> initialization.
        /// </summary>
        public bool IsInitialized { get; private set; }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            DontDestroyOnLoad(gameObject);

            OnInitialize();
            IsInitialized = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// Called once when the singleton instance is created.
        /// Derived managers should perform setup here instead of <see cref="Awake"/>.
        /// </summary>
        protected abstract void OnInitialize();
    }
}
