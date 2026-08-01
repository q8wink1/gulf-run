using GulfRun.Core;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Weapons.Configuration;
using GulfRun.Features.Weapons.Inventory;
using UnityEngine;

namespace GulfRun.Features.Weapons.Authority
{
    /// <summary>
    /// Host-authoritative decision-maker for every Weapon System network
    /// message: validates and resolves Item Box pickups (inventory-full
    /// check, rarity roll, "only one Legendary per match"), validates weapon
    /// activations (does the user actually carry it), and resolves which
    /// participant(s) a use request hits based on its
    /// <see cref="WeaponTargetingType"/>. Exactly the same role
    /// <c>MatchManager</c> plays for match state — every gameplay-facing
    /// system (inventory, effects, debug UI) reacts only to the Confirmed
    /// events this class produces, never to a raw request, so a client can
    /// never grant itself a weapon or fake a hit.
    ///
    /// Persistent (match-spanning) — placed alongside the Sprint 4
    /// Connection/Lobby/Match/Session managers in Boot.unity's
    /// MultiplayerSystems GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WeaponAuthority : Singleton<WeaponAuthority>
    {
        [SerializeField] private WeaponCatalogConfig catalog;

        private IMatchTransport _transport;
        private IRandomSource _random;
        private bool _legendaryGrantedThisMatch;

        protected override void OnInitialize()
        {
            _random = SeededRandom.FromTime();
        }

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.WeaponPickupRequested += HandlePickupRequested;
            _transport.WeaponUseRequested += HandleUseRequested;
            _transport.WeaponHitReported += HandleHitReported;
        }

        private void OnDisable()
        {
            if (_transport == null)
            {
                return;
            }

            _transport.WeaponPickupRequested -= HandlePickupRequested;
            _transport.WeaponUseRequested -= HandleUseRequested;
            _transport.WeaponHitReported -= HandleHitReported;
        }

        /// <summary>Resets "has the Legendary already been granted" for a fresh match. Safe to call any time (e.g. on Create/Leave Match).</summary>
        public void ResetForNewMatch()
        {
            _legendaryGrantedThisMatch = false;
        }

        private void HandlePickupRequested(WeaponPickupRequest request)
        {
            if (_transport == null || !_transport.IsHost || catalog == null)
            {
                return;
            }

            bool inventoryFull = WeaponInventoryManager.Instance != null &&
                                  WeaponInventoryManager.Instance.IsFull(request.CollectorConnectionId);

            WeaponId grantedWeapon = default;
            bool granted = false;

            if (!inventoryFull)
            {
                if (WeaponSpawnRoll.ShouldRollLegendary(_random, catalog.LegendarySpawnChance01, _legendaryGrantedThisMatch))
                {
                    grantedWeapon = catalog.LegendaryWeaponId;
                    granted = true;
                    _legendaryGrantedThisMatch = true;
                }
                else if (WeightedSelector.TrySelect(catalog.GetStandardWeightedOptions(), _random, out WeaponId picked))
                {
                    grantedWeapon = picked;
                    granted = true;
                }
            }

            _transport.ConfirmWeaponPickup(new WeaponPickupEvent(request.BoxId, grantedWeapon, granted, request.CollectorConnectionId, request.TimestampSeconds));
        }

        private void HandleUseRequested(WeaponUseRequest request)
        {
            if (_transport == null || !_transport.IsHost)
            {
                return;
            }

            bool owns = WeaponInventoryManager.Instance != null && WeaponInventoryManager.Instance.Owns(request.UserConnectionId, request.Weapon);
            if (!owns)
            {
                return;
            }

            _transport.ConfirmWeaponUse(request);
            ResolveHits(request);
        }

        private void HandleHitReported(WeaponHitEvent hit)
        {
            // Seam for a future real per-client collision/proximity hit
            // report (once remote avatars are physically networked). Every
            // hit today is already resolved deterministically in
            // ResolveHits from the use request itself, so a reported
            // candidate only needs the host's rubber stamp.
            if (_transport != null && _transport.IsHost)
            {
                _transport.ConfirmWeaponHit(hit);
            }
        }

        private void ResolveHits(WeaponUseRequest request)
        {
            WeaponDefinition definition = catalog != null ? catalog.GetDefinition(request.Weapon) : null;
            if (definition == null)
            {
                return;
            }

            switch (definition.TargetingType)
            {
                case WeaponTargetingType.SelfBuff:
                case WeaponTargetingType.Defensive:
                    _transport.ConfirmWeaponHit(new WeaponHitEvent(request.Weapon, request.UserConnectionId, request.UserConnectionId, request.TimestampSeconds));
                    break;

                case WeaponTargetingType.NearestOpponent:
                    if (request.TargetConnectionId >= 0)
                    {
                        _transport.ConfirmWeaponHit(new WeaponHitEvent(request.Weapon, request.UserConnectionId, request.TargetConnectionId, request.TimestampSeconds));
                    }

                    break;

                case WeaponTargetingType.AreaEffect:
                case WeaponTargetingType.Forward:
                    // No real remote-avatar positions exist yet to test true
                    // proximity/forward-line geometry against (Sprint 4/5
                    // remaining TODO) — every other connected participant is
                    // treated as "in range", which is the documented stand-in
                    // ready to be replaced by a real spatial check once
                    // remote avatars are physically spawned.
                    foreach (MatchParticipant participant in _transport.Participants)
                    {
                        int targetId = participant.Identity.ConnectionId;
                        if (targetId == request.UserConnectionId)
                        {
                            continue;
                        }

                        _transport.ConfirmWeaponHit(new WeaponHitEvent(request.Weapon, request.UserConnectionId, targetId, request.TimestampSeconds));
                    }

                    break;
            }
        }
    }
}
