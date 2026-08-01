using System;
using GulfRun.Core.Backend;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;

namespace GulfRun.Features.Progression
{
    /// <summary>
    /// Applies one resolved reward's local effect — shared by
    /// <c>Missions.MissionManager</c> and <c>Login.LoginRewardManager</c>
    /// (both live in this same assembly, so sharing here does not cross a
    /// Feature boundary — <c>Features.Store.BattlePass.BattlePassManager</c>
    /// intentionally keeps its own separate copy of this same shape of
    /// switch, per this project's "each Feature owns its own reward
    /// application" convention). Extends that Sprint 10 shape with the two
    /// concepts new to Sprint 11: granting Battle Pass XP through
    /// <see cref="IBattlePassXpGrantService"/>, and temporary cosmetic
    /// grants with the "already permanently owned -> alternative reward"
    /// fallback (brief "PERMANENT PURCHASE": "If player already owns
    /// permanent version: Never reward temporary duplicate. Instead
    /// reward: Coins/Small Gems/Alternative reward").
    /// </summary>
    internal static class RewardApplication
    {
        public static void Apply(RewardType type, int amount, CosmeticId cosmeticId, bool isTemporaryCosmeticReward, TemporaryCosmeticDuration temporaryDuration, int fallbackCoinsAmount, string ledgerKey)
        {
            switch (type)
            {
                case RewardType.Coins:
                    EconomyManager.Instance?.AddCoins(amount);
                    break;

                case RewardType.Gems:
                    EconomyManager.Instance?.AddGems(amount);
                    break;

                case RewardType.BattlePassXp:
                    BattlePassXpGrantService.Current?.AddXp(amount);
                    break;

                case RewardType.ExclusiveOutfit:
                case RewardType.ExclusiveEmote:
                case RewardType.VictoryPose:
                case RewardType.ExclusiveSkin:
                case RewardType.LimitedCosmetic:
                    ApplyCosmeticReward(cosmeticId, isTemporaryCosmeticReward, temporaryDuration, fallbackCoinsAmount);
                    break;

                case RewardType.ProfileFrame:
                case RewardType.ChampionEffect:
                case RewardType.Title:
                case RewardType.Badge:
                    // No dedicated inventory slot exists yet for these types
                    // (same Sprint 10 report Remaining TODO) — tracked in
                    // Progression's own generic ledger so ownership is
                    // still real, not just a notification.
                    ProgressionBackendService.Current.GrantProgressionRewardItem(ledgerKey, type);
                    break;
            }
        }

        private static void ApplyCosmeticReward(CosmeticId cosmeticId, bool isTemporary, TemporaryCosmeticDuration duration, int fallbackCoinsAmount)
        {
            if (cosmeticId.IsNone || CosmeticGrantService.Current == null)
            {
                return;
            }

            if (!isTemporary)
            {
                CosmeticGrantService.Current.GrantCosmetic(cosmeticId);
                return;
            }

            if (CosmeticGrantService.Current.OwnsCosmeticPermanently(cosmeticId))
            {
                // Brief: "Never reward temporary duplicate. Instead reward: Coins/Small Gems/Alternative reward."
                EconomyManager.Instance?.AddCoins(fallbackCoinsAmount);
                return;
            }

            double expiresAtSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + duration.ToSeconds();
            CosmeticGrantService.Current.GrantTemporaryCosmetic(cosmeticId, expiresAtSeconds);
        }
    }
}
