namespace GulfRun.Domain
{
    /// <summary>
    /// Pure, deterministic 4-slot spawn formation for a race. All 2D
    /// gameplay (Sprint 2/3) runs on a single ground line with gravity
    /// along -Y, so slots are staggered along X (the run axis) rather than
    /// stacked in Y/altitude — every player still lands on the same ground,
    /// just offset enough at the start line that no two spawn positions
    /// ever coincide. By construction, this guarantees "prevent overlapping"
    /// for the fixed player count without any runtime overlap checking.
    /// </summary>
    public static class MatchSpawnLayout
    {
        public const int MaxSlots = 4;
        private const float SlotSpacing = 0.75f;

        public static NetVector2 GetSpawnOffset(int slotIndex)
        {
            int wrapped = slotIndex < 0 ? 0 : slotIndex % MaxSlots;
            float centeredIndex = wrapped - (MaxSlots - 1) / 2f;
            return new NetVector2(centeredIndex * SlotSpacing, 0f);
        }
    }
}
