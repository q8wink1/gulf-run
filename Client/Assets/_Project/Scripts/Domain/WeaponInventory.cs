using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// One player's carried-weapon inventory: a fixed 2 slots, one weapon
    /// use each, no replacement when full. Pure data/logic — no Unity
    /// dependency — so the exact same rules can be re-run authoritatively on
    /// a future dedicated server and mirrored locally for prediction.
    /// </summary>
    public sealed class WeaponInventory
    {
        public const int MaxSlots = 2;

        private readonly WeaponId?[] _slots = new WeaponId?[MaxSlots];

        public IReadOnlyList<WeaponId?> Slots => _slots;

        public int Count
        {
            get
            {
                int count = 0;
                for (int i = 0; i < MaxSlots; i++)
                {
                    if (_slots[i].HasValue)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public bool IsFull => Count >= MaxSlots;

        public bool Contains(WeaponId weapon)
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                if (_slots[i].HasValue && _slots[i].Value == weapon)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Places <paramref name="weapon"/> into the first empty slot.
        /// Returns false (weapon NOT collected — "the Item Box is lost") if
        /// the inventory is already full. Never replaces an existing weapon.
        /// </summary>
        public bool TryCollect(WeaponId weapon, out int slotIndex)
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                if (!_slots[i].HasValue)
                {
                    _slots[i] = weapon;
                    slotIndex = i;
                    return true;
                }
            }

            slotIndex = -1;
            return false;
        }

        /// <summary>Removes the first slot holding <paramref name="weapon"/> (one-time use — it disappears immediately).</summary>
        public bool TryConsume(WeaponId weapon, out int slotIndex)
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                if (_slots[i].HasValue && _slots[i].Value == weapon)
                {
                    _slots[i] = null;
                    slotIndex = i;
                    return true;
                }
            }

            slotIndex = -1;
            return false;
        }

        public bool TryConsumeSlot(int slotIndex, out WeaponId weapon)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlots || !_slots[slotIndex].HasValue)
            {
                weapon = default;
                return false;
            }

            weapon = _slots[slotIndex].Value;
            _slots[slotIndex] = null;
            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                _slots[i] = null;
            }
        }
    }
}
