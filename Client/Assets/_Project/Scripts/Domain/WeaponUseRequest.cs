namespace GulfRun.Domain
{
    /// <summary>
    /// A weapon-activation message, used both as the client's ask ("I want
    /// to use this weapon") and — once validated — as the authority's
    /// broadcast confirmation (same shape; the authority does not need to
    /// invent new data, only accept or reject). <see cref="TargetConnectionId"/>
    /// is -1 when the targeting type resolves multiple/no explicit targets
    /// (AreaEffect/Forward) — see <c>Features.Weapons.Authority.WeaponAuthority</c>.
    /// </summary>
    public readonly struct WeaponUseRequest
    {
        public readonly WeaponId Weapon;
        public readonly int UserConnectionId;
        public readonly int TargetConnectionId;
        public readonly NetVector2 OriginPosition;
        public readonly double TimestampSeconds;

        public WeaponUseRequest(WeaponId weapon, int userConnectionId, int targetConnectionId, NetVector2 originPosition, double timestampSeconds)
        {
            Weapon = weapon;
            UserConnectionId = userConnectionId;
            TargetConnectionId = targetConnectionId;
            OriginPosition = originPosition;
            TimestampSeconds = timestampSeconds;
        }
    }
}
