using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Spawning
{
    /// <summary>
    /// Assigns each connected participant a unique, non-overlapping spawn
    /// slot (see <see cref="MatchSpawnLayout"/>) for up to
    /// <see cref="MatchSpawnLayout.MaxSlots"/> players. Scene-scoped (resets
    /// on every Gameplay scene reload/restart) since spawn assignment only
    /// matters for the current race. Intentionally computes positions only —
    /// it does not instantiate/move any Player.prefab instance, since no
    /// networked player avatar exists in any scene yet (final gameplay
    /// wiring is explicitly out of scope for this foundation sprint); a
    /// future PlayerSpawnController is the natural consumer of
    /// <see cref="TryGetSpawnPosition"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpawnManager : SceneSingleton<SpawnManager>
    {
        [Tooltip("World-space origin of the start line; spawn offsets are added to this. Defaults to world origin if unset.")]
        [SerializeField] private Transform originTransform;

        private readonly Dictionary<int, int> _connectionIdToSlot = new Dictionary<int, int>();

        /// <summary>Assigns the next free slot to a connection (idempotent — repeated calls for the same connection return the same slot).</summary>
        public int AssignSlot(int connectionId)
        {
            if (_connectionIdToSlot.TryGetValue(connectionId, out int existingSlot))
            {
                return existingSlot;
            }

            int slot = _connectionIdToSlot.Count % MatchSpawnLayout.MaxSlots;
            _connectionIdToSlot[connectionId] = slot;
            return slot;
        }

        public void ReleaseSlot(int connectionId)
        {
            _connectionIdToSlot.Remove(connectionId);
        }

        public bool TryGetSpawnPosition(int connectionId, out Vector2 position)
        {
            if (!_connectionIdToSlot.TryGetValue(connectionId, out int slot))
            {
                position = default;
                return false;
            }

            NetVector2 offset = MatchSpawnLayout.GetSpawnOffset(slot);
            Vector2 origin = originTransform != null ? (Vector2)originTransform.position : Vector2.zero;
            position = new Vector2(origin.x + offset.X, origin.y + offset.Y);
            return true;
        }

        public void ResetAssignments()
        {
            _connectionIdToSlot.Clear();
        }
    }
}
