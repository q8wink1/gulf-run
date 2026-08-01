namespace GulfRun.Core.Services
{
    /// <summary>
    /// The seam <c>Features.Progression</c> (Daily Missions / Login
    /// Rewards) grants Battle Pass XP through instead of ever referencing
    /// <c>Features.Store</c> directly — the same "implement the Core
    /// interface, don't reference the Feature" shape
    /// <see cref="ICosmeticGrantService"/> already established. Implemented
    /// by <c>Features.Store.BattlePass.BattlePassManager</c>.
    /// </summary>
    public interface IBattlePassXpGrantService
    {
        void AddXp(int amount);
    }
}
