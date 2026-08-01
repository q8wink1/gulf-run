namespace GulfRun.Domain
{
    /// <summary>Pure mapping from Locker UI categories to equip slots. Characters is not a slot.</summary>
    public static class LockerCategoryMapping
    {
        public static bool TryGetSlot(LockerCategory category, out CosmeticSlot slot)
        {
            switch (category)
            {
                case LockerCategory.Outfits:
                    slot = CosmeticSlot.Outfit;
                    return true;
                case LockerCategory.Headwear:
                    slot = CosmeticSlot.Hat;
                    return true;
                case LockerCategory.Glasses:
                    slot = CosmeticSlot.Glasses;
                    return true;
                case LockerCategory.VictoryPoses:
                    slot = CosmeticSlot.VictoryPose;
                    return true;
                case LockerCategory.Emotes:
                    slot = CosmeticSlot.Emote;
                    return true;
                case LockerCategory.FootstepEffects:
                    slot = CosmeticSlot.FootstepEffect;
                    return true;
                case LockerCategory.RunningEffects:
                    slot = CosmeticSlot.RunningEffect;
                    return true;
                case LockerCategory.ProfileFrames:
                    slot = CosmeticSlot.ProfileFrame;
                    return true;
                case LockerCategory.Titles:
                    slot = CosmeticSlot.Title;
                    return true;
                default:
                    slot = CosmeticSlot.Outfit;
                    return false;
            }
        }

        public static string DisplayName(LockerCategory category)
        {
            switch (category)
            {
                case LockerCategory.Characters: return "Characters";
                case LockerCategory.Outfits: return "Outfits";
                case LockerCategory.Headwear: return "Headwear";
                case LockerCategory.Glasses: return "Glasses";
                case LockerCategory.VictoryPoses: return "Victory Poses";
                case LockerCategory.Emotes: return "Emotes";
                case LockerCategory.FootstepEffects: return "Footstep Effects";
                case LockerCategory.RunningEffects: return "Running Effects";
                case LockerCategory.ProfileFrames: return "Profile Frames";
                case LockerCategory.Titles: return "Titles";
                default: return category.ToString();
            }
        }
    }
}
