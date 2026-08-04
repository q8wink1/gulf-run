using GulfRun.Core.Managers;
using UnityEngine;

namespace GulfRun.Features.Intro
{
    /// <summary>
    /// After Map Voting, briefly shows the Loading brand screen then loads Gameplay
    /// so all clients "sync" (placeholder) before the race.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LoadingTransitionController : MonoBehaviour
    {
        [SerializeField, Range(0.5f, 6f)] private float holdSeconds = 2.2f;

        private float _elapsed;
        private bool _done;

        private void Update()
        {
            if (_done)
            {
                return;
            }

            _elapsed += Time.unscaledDeltaTime;
            if (_elapsed < holdSeconds)
            {
                return;
            }

            _done = true;
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadGameplay();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GameplaySceneName);
        }
    }
}
