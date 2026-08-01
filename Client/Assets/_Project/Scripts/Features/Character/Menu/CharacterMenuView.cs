using GulfRun.Core.Services;
using GulfRun.Features.Character.Locker;
using UnityEngine;

namespace GulfRun.Features.Character.Menu
{
    /// <summary>
    /// Sprint 8 Character Menu entry point. Sprint 16 moves the full
    /// Character Selection / Locker / Customization experience into
    /// <see cref="LockerView"/>; this component remains on Boot for
    /// scene-GUID stability and forwards open requests / the corner button
    /// to the Locker so Main Menu "Characters"/"Customize" keep working.
    /// </summary>
    public sealed class CharacterMenuView : MonoBehaviour
    {
        private void OnGUI()
        {
            if (!PersistentUiScope.AllowsMainMenuChrome)
            {
                return;
            }

            // Corner toggle lives on LockerView now; keep a thin fallback if
            // LockerView is missing from the scene for any reason.
            if (LockerView.Instance != null)
            {
                return;
            }

            if (GUI.Button(new Rect(10, Screen.height - 46, 160, 34), "Locker (missing)"))
            {
                Debug.LogWarning("LockerView is not present on CharacterSystems — add LockerView to Boot.unity.");
            }
        }
    }
}
