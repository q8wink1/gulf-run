namespace GulfRun.Core.Services
{
    /// <summary>Static service locator for <see cref="IBattlePassXpGrantService"/> — same pattern as <see cref="CosmeticGrantService"/>.</summary>
    public static class BattlePassXpGrantService
    {
        public static IBattlePassXpGrantService Current { get; set; }
    }
}
