using GulfRun.Domain;

namespace GulfRun.Core.Save
{
    /// <summary>
    /// Persistence seam for the local player's Locker selections (Sprint 16).
    /// Implemented by <c>SaveManager</c> via PlayerPrefs — a deliberate
    /// cross-restart exception matching <c>HasSeenIntro</c>, so equip/
    /// character choices survive app restarts without a backend yet.
    /// </summary>
    public interface ILoadoutRepository
    {
        bool TryLoadLoadout(out LoadoutSaveData data);

        void SaveLoadout(LoadoutSaveData data);
    }
}
