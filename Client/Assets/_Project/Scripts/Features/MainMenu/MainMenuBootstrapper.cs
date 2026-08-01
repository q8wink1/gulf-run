using GulfRun.Core.Managers;
using UnityEngine;

namespace GulfRun.Features.MainMenu
{
    /// <summary>
    /// Sprint 13 composition root: the one script the MainMenu scene needs
    /// to kick off lobby music/ambience. Every visual widget
    /// (TopBar/SideMenus/PlayButton/Widgets/Background) is independently
    /// self-contained (each reads its own Core seam and draws itself), so
    /// this bootstrapper's only remaining job is cross-cutting startup:
    /// audio (brief "AUDIO: Soft Gulf inspired music... ambient city
    /// sounds"). Music/ambient clip assignment is intentionally left to
    /// the Inspector (no audio assets exist in this repo yet — see Sprint
    /// 13 report Remaining TODOs), so this safely no-ops until clips are
    /// authored.
    /// </summary>
    public sealed class MainMenuBootstrapper : MonoBehaviour
    {
        [SerializeField] private AudioClip lobbyMusic;
        [SerializeField] private AudioClip ambientCitySound;
        [SerializeField, Range(0f, 1f)] private float lobbyMusicVolume = 0.6f;
        [SerializeField, Range(0f, 1f)] private float ambientVolume = 0.5f;

        private void Start()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            if (lobbyMusic != null)
            {
                AudioManager.Instance.PlayMusic(lobbyMusic, lobbyMusicVolume);
            }

            if (ambientCitySound != null)
            {
                AudioManager.Instance.PlayAmbient(ambientCitySound, ambientVolume);
            }
        }
    }
}
