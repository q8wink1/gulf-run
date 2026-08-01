using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Rewards
{
    /// <summary>
    /// Applies the local player's confirmed race reward to the Coins wallet
    /// exactly once per race — kept separate from <c>RewardScreenView</c> so
    /// the presentation layer never mutates game state (the same UI/
    /// simulation separation <c>CountdownView</c> documents for itself).
    /// </summary>
    public sealed class RaceRewardApplier : MonoBehaviour
    {
        private bool _appliedThisRace;

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            transport.RaceRewardCalculated += HandleRewardCalculated;
            transport.MatchStateChanged += HandleMatchStateChanged;
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null)
            {
                return;
            }

            transport.RaceRewardCalculated -= HandleRewardCalculated;
            transport.MatchStateChanged -= HandleMatchStateChanged;
        }

        private void HandleMatchStateChanged(MatchState state)
        {
            if (state == MatchState.Running)
            {
                _appliedThisRace = false;
            }
        }

        private void HandleRewardCalculated(RaceRewardBreakdown reward)
        {
            if (_appliedThisRace)
            {
                return;
            }

            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null || reward.ConnectionId != transport.LocalConnectionId)
            {
                return;
            }

            _appliedThisRace = true;
            EconomyManager.Instance?.AddCoins(reward.TotalReward);
        }
    }
}
