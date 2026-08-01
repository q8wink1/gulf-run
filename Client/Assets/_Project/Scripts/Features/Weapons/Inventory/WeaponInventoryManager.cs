using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Weapons.Configuration;
using UnityEngine;

namespace GulfRun.Features.Weapons.Inventory
{
    /// <summary>
    /// Per-connection Weapon Inventory (max 2 slots, no replacement — see
    /// <see cref="WeaponInventory"/>), kept in sync purely by listening to
    /// <see cref="IMatchTransport"/>'s confirmed pickup/use events, exactly
    /// like <c>LobbyManager</c> keeps the roster in sync from participant
    /// events. Persistent (match-spanning) — placed alongside the Sprint 4
    /// Connection/Lobby/Match/Session managers in Boot.unity's
    /// MultiplayerSystems GameObject.
    ///
    /// Also owns "use a carried weapon" for the local player
    /// (<see cref="TryUseLocalSlot"/>) and the short per-player activation
    /// cooldown, since both are the same "local inventory" responsibility.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WeaponInventoryManager : Singleton<WeaponInventoryManager>
    {
        [SerializeField] private WeaponCatalogConfig catalog;

        private readonly Dictionary<int, WeaponInventory> _inventories = new Dictionary<int, WeaponInventory>();
        private readonly Dictionary<int, double> _lastUseTimeSeconds = new Dictionary<int, double>();

        private IMatchTransport _transport;

        /// <summary>Raised after a connection's inventory changes (pickup granted or a weapon consumed).</summary>
        public event Action<int> InventoryChanged;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.WeaponPickupConfirmed += HandlePickupConfirmed;
            _transport.WeaponUseConfirmed += HandleUseConfirmed;
            _transport.ParticipantLeft += HandleParticipantLeft;
        }

        private void OnDisable()
        {
            if (_transport == null)
            {
                return;
            }

            _transport.WeaponPickupConfirmed -= HandlePickupConfirmed;
            _transport.WeaponUseConfirmed -= HandleUseConfirmed;
            _transport.ParticipantLeft -= HandleParticipantLeft;
        }

        public bool IsFull(int connectionId) => GetOrCreate(connectionId).IsFull;

        public bool Owns(int connectionId, WeaponId weapon) => GetOrCreate(connectionId).Contains(weapon);

        public IReadOnlyList<WeaponId?> GetSlots(int connectionId) => GetOrCreate(connectionId).Slots;

        public bool IsOnCooldown(int connectionId) =>
            _lastUseTimeSeconds.TryGetValue(connectionId, out double last) && Time.timeAsDouble - last < CooldownSecondsFor(connectionId);

        /// <summary>Clears every tracked inventory, e.g. on leaving a match. Does not touch the network — purely local bookkeeping.</summary>
        public void ClearAll()
        {
            _inventories.Clear();
            _lastUseTimeSeconds.Clear();
        }

        /// <summary>
        /// Attempts to activate the weapon carried in the local player's
        /// <paramref name="slotIndex"/>. Resolves a default target for
        /// single-target weapon types (self for Defensive/SelfBuff, the
        /// first other connected participant for NearestOpponent — true
        /// "nearest" needs a real remote-avatar position, which does not
        /// exist yet; see Sprint 4/5 remaining TODOs). Sends the request to
        /// the authority; the slot is only actually cleared once
        /// <see cref="HandleUseConfirmed"/> fires.
        /// </summary>
        public bool TryUseLocalSlot(int slotIndex)
        {
            if (_transport == null)
            {
                return false;
            }

            int localId = _transport.LocalConnectionId;
            if (IsOnCooldown(localId))
            {
                return false;
            }

            WeaponInventory inventory = GetOrCreate(localId);
            IReadOnlyList<WeaponId?> slots = inventory.Slots;
            if (slotIndex < 0 || slotIndex >= slots.Count || !slots[slotIndex].HasValue)
            {
                return false;
            }

            WeaponId weapon = slots[slotIndex].Value;
            WeaponDefinition definition = catalog != null ? catalog.GetDefinition(weapon) : null;
            int targetConnectionId = ResolveDefaultTarget(definition, localId, _transport);

            Vector2 origin = LocalPlayerStateService.Current != null ? LocalPlayerStateService.Current.Position : Vector2.zero;
            var request = new WeaponUseRequest(weapon, localId, targetConnectionId, new NetVector2(origin.x, origin.y), Time.timeAsDouble);

            _lastUseTimeSeconds[localId] = Time.timeAsDouble;
            _transport.RequestWeaponUse(request);
            return true;
        }

        private float CooldownSecondsFor(int connectionId)
        {
            // Cooldown is a per-activation client-side spam guard, not part
            // of the authoritative weapon rules, so any equipped weapon's
            // own CooldownSeconds is a reasonable default even before the
            // authority confirms which weapon (if any) is being fired.
            WeaponInventory inventory = GetOrCreate(connectionId);
            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                if (inventory.Slots[i].HasValue && catalog != null)
                {
                    WeaponDefinition definition = catalog.GetDefinition(inventory.Slots[i].Value);
                    if (definition != null)
                    {
                        return definition.CooldownSeconds;
                    }
                }
            }

            return 0.5f;
        }

        private static int ResolveDefaultTarget(WeaponDefinition definition, int localId, IMatchTransport transport)
        {
            if (definition == null)
            {
                return -1;
            }

            switch (definition.TargetingType)
            {
                case WeaponTargetingType.SelfBuff:
                case WeaponTargetingType.Defensive:
                    return localId;

                case WeaponTargetingType.NearestOpponent:
                    foreach (MatchParticipant participant in transport.Participants)
                    {
                        if (participant.Identity.ConnectionId != localId)
                        {
                            return participant.Identity.ConnectionId;
                        }
                    }

                    return -1;

                default:
                    // AreaEffect/Forward: every in-range opponent is resolved
                    // broadcast-style by WeaponAuthority, no single target here.
                    return -1;
            }
        }

        private void HandlePickupConfirmed(WeaponPickupEvent confirmed)
        {
            if (!confirmed.Granted)
            {
                return;
            }

            WeaponInventory inventory = GetOrCreate(confirmed.CollectorConnectionId);
            if (inventory.TryCollect(confirmed.Weapon, out _))
            {
                InventoryChanged?.Invoke(confirmed.CollectorConnectionId);
            }
        }

        private void HandleUseConfirmed(WeaponUseRequest confirmed)
        {
            WeaponInventory inventory = GetOrCreate(confirmed.UserConnectionId);
            if (inventory.TryConsume(confirmed.Weapon, out _))
            {
                InventoryChanged?.Invoke(confirmed.UserConnectionId);
            }
        }

        private void HandleParticipantLeft(int connectionId, DisconnectReason reason)
        {
            _inventories.Remove(connectionId);
            _lastUseTimeSeconds.Remove(connectionId);
        }

        private WeaponInventory GetOrCreate(int connectionId)
        {
            if (!_inventories.TryGetValue(connectionId, out WeaponInventory inventory))
            {
                inventory = new WeaponInventory();
                _inventories[connectionId] = inventory;
            }

            return inventory;
        }
    }
}
