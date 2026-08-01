using System.Collections.Generic;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// A small registry mapping each <see cref="MenuScreen"/> to whichever
    /// existing Feature's <see cref="IMenuScreenOpener"/> currently owns it
    /// — the "many small screens, each self-registering" generalization of
    /// the single-interface locator pattern every other Sprint uses (see
    /// <see cref="MapContextService"/>, <see cref="LocalLoadoutProviderService"/>).
    /// Registering the same screen twice simply replaces the previous
    /// opener (harmless — only one scene's instance of a given screen is
    /// ever loaded at a time under this project's current single-menu-scene
    /// setup).
    /// </summary>
    public static class MenuScreenRouter
    {
        private static readonly Dictionary<MenuScreen, IMenuScreenOpener> Openers = new Dictionary<MenuScreen, IMenuScreenOpener>();

        public static void Register(MenuScreen screen, IMenuScreenOpener opener)
        {
            if (opener != null)
            {
                Openers[screen] = opener;
            }
        }

        public static void Unregister(MenuScreen screen, IMenuScreenOpener opener)
        {
            if (Openers.TryGetValue(screen, out IMenuScreenOpener current) && ReferenceEquals(current, opener))
            {
                Openers.Remove(screen);
            }
        }

        /// <summary>Returns false if no Feature has registered an opener for <paramref name="screen"/> yet (e.g. its scene instance has not loaded).</summary>
        public static bool TryOpen(MenuScreen screen)
        {
            if (Openers.TryGetValue(screen, out IMenuScreenOpener opener))
            {
                opener.OpenScreen(screen);
                return true;
            }

            return false;
        }
    }
}
