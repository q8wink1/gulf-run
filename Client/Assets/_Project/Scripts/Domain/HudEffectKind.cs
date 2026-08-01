namespace GulfRun.Domain
{
    /// <summary>
    /// Player-facing HUD vocabulary for active status effects. Maps from the
    /// shared <see cref="WeaponEffectFlags"/> bitfield into a small set of
    /// readable chips (Speed Boost / Shield / Blindness / Sand Slow / Coffee
    /// Stun) so presentation never branches on raw flags.
    /// </summary>
    public enum HudEffectKind
    {
        SpeedBoost,
        Shield,
        Blindness,
        SandSlow,
        CoffeeStun
    }

    /// <summary>Pure flag→HUD-kind resolver. One flag can contribute at most one kind; combined effects yield multiple snapshots at the call site.</summary>
    public static class HudEffectKindResolver
    {
        public static bool TryResolve(WeaponEffectFlags flags, out HudEffectKind kind)
        {
            if ((flags & WeaponEffectFlags.SpeedBoost) != 0)
            {
                kind = HudEffectKind.SpeedBoost;
                return true;
            }

            if ((flags & WeaponEffectFlags.Shield) != 0)
            {
                kind = HudEffectKind.Shield;
                return true;
            }

            if ((flags & WeaponEffectFlags.VisionReduced) != 0)
            {
                kind = HudEffectKind.Blindness;
                return true;
            }

            if ((flags & (WeaponEffectFlags.Slow | WeaponEffectFlags.TractionLoss)) != 0)
            {
                kind = HudEffectKind.SandSlow;
                return true;
            }

            if ((flags & (WeaponEffectFlags.Pause | WeaponEffectFlags.Stun | WeaponEffectFlags.Knockdown)) != 0)
            {
                kind = HudEffectKind.CoffeeStun;
                return true;
            }

            kind = default;
            return false;
        }

        public static string ResolveLabel(HudEffectKind kind) => kind switch
        {
            HudEffectKind.SpeedBoost => "SPEED",
            HudEffectKind.Shield => "SHIELD",
            HudEffectKind.Blindness => "BLIND",
            HudEffectKind.SandSlow => "SLOW",
            HudEffectKind.CoffeeStun => "STUN",
            _ => string.Empty
        };

        /// <summary>Color-blind-friendly shape tags (not color-only cues).</summary>
        public static string ResolveShapeTag(HudEffectKind kind) => kind switch
        {
            HudEffectKind.SpeedBoost => "▲",
            HudEffectKind.Shield => "◆",
            HudEffectKind.Blindness => "◌",
            HudEffectKind.SandSlow => "▼",
            HudEffectKind.CoffeeStun => "■",
            _ => "•"
        };
    }
}
