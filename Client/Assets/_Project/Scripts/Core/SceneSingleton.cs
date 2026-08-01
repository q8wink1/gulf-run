using UnityEngine;

namespace GulfRun.Core
{
    /// <summary>
    /// Generic base class for MonoBehaviour services that must have a single
    /// active instance within the CURRENT scene only. Unlike
    /// <see cref="Singleton{T}"/>, this does NOT call DontDestroyOnLoad — the
    /// correct choice for gameplay-session-scoped systems (endless-runner
    /// world generation, speed, distance, scoring, game loop, ...) that must
    /// reset cleanly whenever the Gameplay scene reloads (e.g. Restart)
    /// instead of persisting stale state across attempts.
    /// </summary>
    /// <typeparam name="T">Concrete service type.</typeparam>
    public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
    {
        private static T _instance;

        /// <summary>
        /// Scene-local access point for the service instance. Returns null if
        /// no instance exists in the currently loaded scene.
        /// </summary>
        public static T Instance => _instance;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
