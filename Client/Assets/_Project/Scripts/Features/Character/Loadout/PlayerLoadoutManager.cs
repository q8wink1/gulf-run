using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using UnityEngine;

namespace GulfRun.Features.Character.Loadout
{
    /// <summary>
    /// Composition root for the Character &amp; Customization system — the
    /// same role <c>SessionManager</c> plays for Multiplayer and
    /// <c>RaceFinishAuthority</c> plays for Race Finish. Owns the local
    /// player's live <see cref="PlayerLoadout"/> (Character + the
    /// account-locked Country + every equipped <see cref="CosmeticId"/>)
    /// and <see cref="CosmeticInventory"/> (permanent ownership), applies
    /// the "selecting a country automatically applies its national clothing"
    /// rule the instant an account exists, brokers Gem-funded unlocks
    /// through <see cref="EconomyManager"/>, and keeps every other
    /// participant's loadout mirrored via <see cref="IMatchTransport.LoadoutChanged"/>
    /// so lobby/race/podium presentation can show everyone's chosen
    /// character/outfit — never just the local player's.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerLoadoutManager : Singleton<PlayerLoadoutManager>, ILocalLoadoutProvider, ICosmeticGrantService
    {
        [SerializeField] private CharacterCatalogConfig characterCatalog;
        [SerializeField] private CosmeticCatalogConfig cosmeticCatalog;

        private readonly CosmeticInventory _localInventory = new CosmeticInventory();
        private readonly Dictionary<int, PlayerLoadout> _remoteLoadouts = new Dictionary<int, PlayerLoadout>();

        private PlayerLoadout _localLoadout;
        private bool _localInitialized;

        public CharacterCatalogConfig CharacterCatalog => characterCatalog;
        public CosmeticCatalogConfig CosmeticCatalog => cosmeticCatalog;

        /// <summary>Null until an account exists (see <see cref="SaveManager.HasAccount"/>) — Account Creation always runs first.</summary>
        public PlayerLoadout LocalLoadout => _localLoadout;

        public CosmeticInventory LocalInventory => _localInventory;

        public IReadOnlyDictionary<int, PlayerLoadout> RemoteLoadouts => _remoteLoadouts;

        protected override void OnInitialize()
        {
            // Local loadout initialization is deferred to Update/TryInitializeFromAccount
            // since it depends on Account Creation (SaveManager.HasAccount), which may
            // not have happened yet when this singleton wakes up.
        }

        // --- Sprint 9: ILocalLoadoutProvider — lets Features.Online's
        // ProfileManager show "Current Character"/"Current Outfit" on the
        // Player Profile screen with zero compile-time reference to this
        // (Features.Character) assembly. See Core.Services.ILocalLoadoutProvider.

        CharacterId ILocalLoadoutProvider.CurrentCharacterId => _localLoadout != null ? _localLoadout.Character : CharacterId.None;

        string ILocalLoadoutProvider.CurrentCharacterDisplayName
        {
            get
            {
                if (_localLoadout == null || characterCatalog == null)
                {
                    return string.Empty;
                }

                CharacterDefinition definition = characterCatalog.GetDefinition(_localLoadout.Character);
                return definition != null ? definition.DisplayName : string.Empty;
            }
        }

        CosmeticId ILocalLoadoutProvider.CurrentOutfitId => _localLoadout != null ? _localLoadout.GetEquipped(CosmeticSlot.Outfit) : CosmeticId.None;

        string ILocalLoadoutProvider.CurrentOutfitDisplayName
        {
            get
            {
                CosmeticId outfit = _localLoadout != null ? _localLoadout.GetEquipped(CosmeticSlot.Outfit) : CosmeticId.None;
                if (outfit.IsNone || cosmeticCatalog == null)
                {
                    return string.Empty;
                }

                return cosmeticCatalog.TryGetEntry(outfit, out CosmeticCatalogConfig.CosmeticEntry entry) ? entry.DisplayName : string.Empty;
            }
        }

        GulfCountry ILocalLoadoutProvider.Country => _localLoadout != null ? _localLoadout.Country : GulfCountry.SaudiArabia;

        // --- Sprint 10: ICosmeticGrantService — lets Features.Store credit a
        // purchased Outfit/Emote/Victory Pose straight into the real
        // CosmeticInventory with zero compile-time reference to this
        // (Features.Character) assembly. See Core.Services.ICosmeticGrantService.
        //
        // Sprint 11 addition: temporary (expiring) ownership for Daily
        // Mission / Login Reward grants — see Domain.CosmeticInventory
        // remarks for the permanent-always-wins upgrade rule.

        bool ICosmeticGrantService.OwnsCosmetic(CosmeticId id) => _localInventory.Owns(id);

        bool ICosmeticGrantService.OwnsCosmeticPermanently(CosmeticId id) => _localInventory.OwnsPermanently(id);

        bool ICosmeticGrantService.GrantCosmetic(CosmeticId id)
        {
            if (id.IsNone)
            {
                return false;
            }

            _localInventory.Grant(id);
            return true;
        }

        bool ICosmeticGrantService.GrantTemporaryCosmetic(CosmeticId id, double expiresAtSeconds) => _localInventory.GrantTemporary(id, expiresAtSeconds);

        IReadOnlyList<CosmeticId> ICosmeticGrantService.GetOwnedCosmetics()
        {
            var owned = new List<CosmeticId>(_localInventory.OwnedIds.Count + _localInventory.TemporaryOwnedIds.Count);
            foreach (string id in _localInventory.OwnedIds)
            {
                owned.Add(new CosmeticId(id));
            }

            foreach (string id in _localInventory.TemporaryOwnedIds)
            {
                owned.Add(new CosmeticId(id));
            }

            return owned;
        }

        IReadOnlyList<TemporaryCosmeticOwnership> ICosmeticGrantService.GetTemporaryCosmetics()
        {
            var temporary = new List<TemporaryCosmeticOwnership>(_localInventory.TemporaryOwnedIds.Count);
            foreach (string idValue in _localInventory.TemporaryOwnedIds)
            {
                var id = new CosmeticId(idValue);
                if (_localInventory.TryGetTemporaryExpiry(id, out double expiresAtSeconds))
                {
                    temporary.Add(new TemporaryCosmeticOwnership(id, 0d, expiresAtSeconds));
                }
            }

            return temporary;
        }

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.LoadoutChanged += HandleLoadoutChanged;
            transport.ParticipantJoined += HandleParticipantJoined;
            transport.MatchStateChanged += HandleMatchStateChanged;
            LocalLoadoutProviderService.Current = this;
            CosmeticGrantService.Current = this;
        }

        private void OnDisable()
        {
            if (LocalLoadoutProviderService.Current == (ILocalLoadoutProvider)this)
            {
                LocalLoadoutProviderService.Current = null;
            }

            if (CosmeticGrantService.Current == (ICosmeticGrantService)this)
            {
                CosmeticGrantService.Current = null;
            }

            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null)
            {
                return;
            }

            transport.LoadoutChanged -= HandleLoadoutChanged;
            transport.ParticipantJoined -= HandleParticipantJoined;
            transport.MatchStateChanged -= HandleMatchStateChanged;
        }

        private static readonly CosmeticSlot[] AllCosmeticSlots = (CosmeticSlot[])Enum.GetValues(typeof(CosmeticSlot));

        private double _nextTemporaryExpiryCheckAtSeconds;

        private void Update()
        {
            if (!_localInitialized)
            {
                TryInitializeFromAccount();
            }

            TickTemporaryCosmeticExpirations();
        }

        /// <summary>
        /// Sprint 11 "TEMPORARY COSMETICS: When expired: Item is
        /// automatically removed." Checked on a throttled interval (real
        /// time is only meaningful to the second here, and this only ever
        /// does real work once every couple of days per grant) rather than
        /// every frame, for "Optimized mobile performance" (brief
        /// "PERFORMANCE"). Uses <see cref="Time.timeAsDouble"/> purely to
        /// throttle the check cadence — the actual expiry comparison inside
        /// <see cref="Domain.CosmeticInventory.RemoveExpired"/> uses
        /// real-world (Unix epoch) seconds.
        /// </summary>
        private void TickTemporaryCosmeticExpirations()
        {
            if (Time.timeAsDouble < _nextTemporaryExpiryCheckAtSeconds)
            {
                return;
            }

            _nextTemporaryExpiryCheckAtSeconds = Time.timeAsDouble + 5d;

            List<string> expiredIds = _localInventory.RemoveExpired(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (expiredIds.Count == 0)
            {
                return;
            }

            bool anyUnequipped = false;
            for (int i = 0; i < expiredIds.Count; i++)
            {
                if (UnequipIfCurrentlyEquipped(new CosmeticId(expiredIds[i])))
                {
                    anyUnequipped = true;
                }
            }

            if (anyUnequipped)
            {
                BroadcastLocalLoadoutIfActive();
            }
        }

        private bool UnequipIfCurrentlyEquipped(CosmeticId id)
        {
            if (_localLoadout == null)
            {
                return false;
            }

            bool unequippedAny = false;
            for (int i = 0; i < AllCosmeticSlots.Length; i++)
            {
                CosmeticSlot slot = AllCosmeticSlots[i];
                if (_localLoadout.GetEquipped(slot) == id)
                {
                    _localLoadout.Equip(slot, CosmeticId.None);
                    unequippedAny = true;
                }
            }

            return unequippedAny;
        }

        /// <summary>Freely switch to any of the 12 (or more) unlocked characters. "Changing character NEVER changes the selected country" — Country is untouched here.</summary>
        public bool SelectCharacter(CharacterId characterId)
        {
            if (_localLoadout == null || characterCatalog == null || characterCatalog.GetDefinition(characterId) == null)
            {
                return false;
            }

            _localLoadout.SetCharacter(characterId);
            BroadcastLocalLoadoutIfActive();
            return true;
        }

        /// <summary>Unlocks a Premium Cosmetic with Gems (Traditional Outfits are never unlocked this way — they are auto-granted for free). Returns false if already owned is impossible to reach, unaffordable, or unknown.</summary>
        public bool TryUnlockCosmetic(CosmeticId cosmeticId)
        {
            if (cosmeticCatalog == null || !cosmeticCatalog.TryGetEntry(cosmeticId, out CosmeticCatalogConfig.CosmeticEntry entry))
            {
                return false;
            }

            if (_localInventory.Owns(cosmeticId))
            {
                return true;
            }

            if (entry.IsTraditionalOutfit || EconomyManager.Instance == null || !EconomyManager.Instance.TrySpendGems(entry.GemPrice))
            {
                return false;
            }

            _localInventory.Grant(cosmeticId);
            return true;
        }

        /// <summary>Equips an owned cosmetic into a slot; refuses anything not owned (COS-OWN-001 — unlocked cosmetics are permanently owned, and only owned cosmetics may ever be equipped).</summary>
        public bool EquipCosmetic(CosmeticSlot slot, CosmeticId cosmeticId)
        {
            if (_localLoadout == null || (!cosmeticId.IsNone && !_localInventory.Owns(cosmeticId)))
            {
                return false;
            }

            _localLoadout.Equip(slot, cosmeticId);
            BroadcastLocalLoadoutIfActive();
            return true;
        }

        public bool TryGetRemoteLoadout(int connectionId, out PlayerLoadout loadout) => _remoteLoadouts.TryGetValue(connectionId, out loadout);

        private void TryInitializeFromAccount()
        {
            if (SaveManager.Instance == null || !SaveManager.Instance.HasAccount)
            {
                return;
            }

            PlayerAccount account = SaveManager.Instance.GetAccount();
            CharacterId defaultCharacter = characterCatalog != null ? characterCatalog.DefaultCharacterId : CharacterId.None;
            _localLoadout = new PlayerLoadout(-1, defaultCharacter, account.Country);

            // "Selecting a country automatically applies its national
            // clothing to any selected character" — and since Country is
            // permanent, this only ever needs to run once, right here.
            CosmeticId traditionalOutfit = cosmeticCatalog != null ? cosmeticCatalog.GetTraditionalOutfitId(account.Country) : CosmeticId.None;
            if (!traditionalOutfit.IsNone)
            {
                _localInventory.Grant(traditionalOutfit);
                _localLoadout.Equip(CosmeticSlot.Outfit, traditionalOutfit);
            }

            _localInitialized = true;
            BroadcastLocalLoadoutIfActive();
        }

        private void HandleParticipantJoined(MatchParticipant participant)
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null && participant.Identity.ConnectionId == transport.LocalConnectionId)
            {
                BroadcastLocalLoadoutIfActive();
            }
        }

        private void HandleMatchStateChanged(MatchState state)
        {
            if (state == MatchState.Waiting)
            {
                BroadcastLocalLoadoutIfActive();
            }
        }

        private void HandleLoadoutChanged(PlayerLoadout loadout)
        {
            if (loadout == null)
            {
                return;
            }

            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null && loadout.ConnectionId == transport.LocalConnectionId)
            {
                // Our own broadcast echoing back through the loopback transport — already reflected in _localLoadout.
                return;
            }

            _remoteLoadouts[loadout.ConnectionId] = loadout.Clone();
        }

        private void BroadcastLocalLoadoutIfActive()
        {
            if (_localLoadout == null)
            {
                return;
            }

            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null || !transport.IsActive)
            {
                return;
            }

            _localLoadout.SetConnectionId(transport.LocalConnectionId);
            transport.SetLocalLoadout(_localLoadout.Clone());
        }
    }
}
