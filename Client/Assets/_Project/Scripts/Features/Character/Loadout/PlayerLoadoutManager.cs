using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
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
    public sealed class PlayerLoadoutManager : Singleton<PlayerLoadoutManager>
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

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.LoadoutChanged += HandleLoadoutChanged;
            transport.ParticipantJoined += HandleParticipantJoined;
            transport.MatchStateChanged += HandleMatchStateChanged;
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null)
            {
                return;
            }

            transport.LoadoutChanged -= HandleLoadoutChanged;
            transport.ParticipantJoined -= HandleParticipantJoined;
            transport.MatchStateChanged -= HandleMatchStateChanged;
        }

        private void Update()
        {
            if (!_localInitialized)
            {
                TryInitializeFromAccount();
            }
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
