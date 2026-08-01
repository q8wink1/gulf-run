using System;

namespace GulfRun.Domain
{
    /// <summary>
    /// One player's live customization state: which character is active,
    /// their (immutable, account-locked) country, and which
    /// <see cref="CosmeticId"/> is equipped in every <see cref="CosmeticSlot"/>
    /// — this is exactly the "Character, Country, Current Outfit, Current
    /// Cosmetics, Victory Pose" set the Sprint 8 brief requires synchronized.
    /// A mutable class (matching <see cref="WeaponInventory"/>'s style) for
    /// the local player's own live-edited state; call <see cref="Clone"/>
    /// before handing a copy to <c>IMatchTransport.SetLocalLoadout</c> or
    /// storing a remote player's received snapshot, so no two owners ever
    /// alias the same mutable slot array.
    /// </summary>
    public sealed class PlayerLoadout
    {
        private static readonly int SlotCount = Enum.GetValues(typeof(CosmeticSlot)).Length;

        public int ConnectionId { get; private set; }
        public CharacterId Character { get; private set; }
        public GulfCountry Country { get; private set; }

        private readonly CosmeticId[] _equippedBySlot;

        public PlayerLoadout(int connectionId, CharacterId character, GulfCountry country)
        {
            ConnectionId = connectionId;
            Character = character;
            Country = country;
            _equippedBySlot = new CosmeticId[SlotCount];
        }

        public void SetConnectionId(int connectionId) => ConnectionId = connectionId;

        public void SetCharacter(CharacterId character) => Character = character;

        public CosmeticId GetEquipped(CosmeticSlot slot) => _equippedBySlot[(int)slot];

        public void Equip(CosmeticSlot slot, CosmeticId cosmetic) => _equippedBySlot[(int)slot] = cosmetic;

        /// <summary>Deep copy — the safe way to pass this loadout across the network seam or store a remote player's latest snapshot.</summary>
        public PlayerLoadout Clone()
        {
            var copy = new PlayerLoadout(ConnectionId, Character, Country);
            Array.Copy(_equippedBySlot, copy._equippedBySlot, _equippedBySlot.Length);
            return copy;
        }
    }
}
