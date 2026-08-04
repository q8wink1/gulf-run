using System;
using System.Collections.Generic;

namespace GulfRun.Features.Multiplayer.Matchmaking
{
    /// <summary>
    /// Offline mock of a public Quick Play room directory. Prefer fuller rooms
    /// first (3/4 → 2/4 → 1/4); create a new room when none are available.
    /// Replaced by a real matchmaking service when a multi-machine transport lands.
    /// </summary>
    public sealed class MockPublicRoomDirectory
    {
        public readonly struct PublicRoomOffer
        {
            public readonly string RoomId;
            public readonly int OccupiedSeats;
            public readonly int MaxSeats;

            public PublicRoomOffer(string roomId, int occupiedSeats, int maxSeats)
            {
                RoomId = roomId;
                OccupiedSeats = occupiedSeats;
                MaxSeats = maxSeats;
            }

            public int OpenSeats => MaxSeats - OccupiedSeats;
        }

        private readonly List<PublicRoomOffer> _rooms = new List<PublicRoomOffer>();
        private readonly System.Random _random = new System.Random();
        private int _nextRoomNumber = 1;

        public void SeedDemoRooms(int maxSeats)
        {
            _rooms.Clear();
            // Deterministic-ish demo pool so Quick Play can exercise join priority.
            TryAddRoom(3, maxSeats);
            TryAddRoom(2, maxSeats);
            TryAddRoom(1, maxSeats);
            if (_random.NextDouble() < 0.35d)
            {
                TryAddRoom(3, maxSeats);
            }
        }

        public bool TryFindBestJoinableRoom(int maxSeats, out PublicRoomOffer offer)
        {
            offer = default;
            int bestOccupied = -1;
            int bestIndex = -1;

            for (int i = 0; i < _rooms.Count; i++)
            {
                PublicRoomOffer room = _rooms[i];
                if (room.MaxSeats != maxSeats || room.OccupiedSeats <= 0 || room.OccupiedSeats >= maxSeats)
                {
                    continue;
                }

                // Priority: 3/4 first, then 2/4, then 1/4 (highest occupancy that still has a seat).
                if (room.OccupiedSeats > bestOccupied)
                {
                    bestOccupied = room.OccupiedSeats;
                    bestIndex = i;
                }
            }

            if (bestIndex < 0)
            {
                return false;
            }

            offer = _rooms[bestIndex];
            _rooms.RemoveAt(bestIndex);
            return true;
        }

        public PublicRoomOffer RegisterNewRoom(int occupiedSeats, int maxSeats)
        {
            var offer = new PublicRoomOffer("QP-" + _nextRoomNumber.ToString("D4"), occupiedSeats, maxSeats);
            _nextRoomNumber++;
            if (occupiedSeats > 0 && occupiedSeats < maxSeats)
            {
                _rooms.Add(offer);
            }

            return offer;
        }

        public void NotifyOccupancy(string roomId, int occupiedSeats, int maxSeats)
        {
            for (int i = _rooms.Count - 1; i >= 0; i--)
            {
                if (_rooms[i].RoomId != roomId)
                {
                    continue;
                }

                if (occupiedSeats <= 0 || occupiedSeats >= maxSeats)
                {
                    _rooms.RemoveAt(i);
                }
                else
                {
                    _rooms[i] = new PublicRoomOffer(roomId, occupiedSeats, maxSeats);
                }

                return;
            }

            if (occupiedSeats > 0 && occupiedSeats < maxSeats)
            {
                _rooms.Add(new PublicRoomOffer(roomId, occupiedSeats, maxSeats));
            }
        }

        private void TryAddRoom(int occupied, int maxSeats)
        {
            if (occupied <= 0 || occupied >= maxSeats)
            {
                return;
            }

            _rooms.Add(new PublicRoomOffer("QP-" + _nextRoomNumber.ToString("D4"), occupied, maxSeats));
            _nextRoomNumber++;
        }
    }

    /// <summary>Result of a mock Quick Play search.</summary>
    public enum QuickPlaySearchOutcome
    {
        None = 0,
        JoinedExistingRoom = 1,
        CreatedNewRoom = 2
    }
}
