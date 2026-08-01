using System.Collections.Generic;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Connection-id-keyed lookup of every live <see cref="IPlayerStatusEffectReceiver"/>
    /// in the current scene. Unlike <see cref="LocalPlayerStateService"/>
    /// (exactly one local provider), multiple players can each register their
    /// own receiver — today that is only ever the local player's, since no
    /// networked remote player avatar is spawned yet (see Sprint 4 remaining
    /// TODOs); registering by connection id keeps this ready for that without
    /// any change here once remote avatars exist.
    /// </summary>
    public static class PlayerStatusEffectRegistry
    {
        private static readonly Dictionary<int, IPlayerStatusEffectReceiver> _receivers = new Dictionary<int, IPlayerStatusEffectReceiver>();

        public static void Register(int connectionId, IPlayerStatusEffectReceiver receiver)
        {
            if (receiver != null)
            {
                _receivers[connectionId] = receiver;
            }
        }

        public static void Unregister(int connectionId, IPlayerStatusEffectReceiver receiver)
        {
            if (_receivers.TryGetValue(connectionId, out IPlayerStatusEffectReceiver existing) && ReferenceEquals(existing, receiver))
            {
                _receivers.Remove(connectionId);
            }
        }

        public static bool TryGet(int connectionId, out IPlayerStatusEffectReceiver receiver) =>
            _receivers.TryGetValue(connectionId, out receiver);
    }
}
