using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Progression.Cosmetics
{
    /// <summary>
    /// Sprint 11 "NOTIFICATIONS: Notify player when ... Temporary item
    /// expiring soon." Auto-removal itself is
    /// <c>Features.Character.Loadout.PlayerLoadoutManager</c>'s job (it
    /// owns the real <see cref="Domain.CosmeticInventory"/>); this class
    /// only reads <see cref="ICosmeticGrantService.GetTemporaryCosmetics"/>
    /// on a throttled interval and raises one notification per item the
    /// first time its remaining time drops at-or-below
    /// <see cref="expiringSoonThresholdSeconds"/>, tracked so the same item
    /// is never warned about twice for the same grant.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TemporaryCosmeticExpiryWatcher : Singleton<TemporaryCosmeticExpiryWatcher>
    {
        [Tooltip("How soon before expiry (in seconds) the 'expiring soon' notification fires. Default 6 hours.")]
        [SerializeField] private double expiringSoonThresholdSeconds = 6 * 60 * 60;

        [Tooltip("How often (in seconds of game time) to re-check every temporary grant.")]
        [SerializeField] private float checkIntervalSeconds = 30f;

        private readonly HashSet<string> _warnedForExpiry = new HashSet<string>();
        private float _nextCheckAtGameSeconds;

        protected override void OnInitialize()
        {
        }

        private void Update()
        {
            if (Time.time < _nextCheckAtGameSeconds)
            {
                return;
            }

            _nextCheckAtGameSeconds = Time.time + checkIntervalSeconds;
            CheckExpiringSoon();
        }

        private void CheckExpiringSoon()
        {
            if (CosmeticGrantService.Current == null)
            {
                return;
            }

            IReadOnlyList<TemporaryCosmeticOwnership> temporary = CosmeticGrantService.Current.GetTemporaryCosmetics();
            if (temporary.Count == 0)
            {
                return;
            }

            double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            for (int i = 0; i < temporary.Count; i++)
            {
                TemporaryCosmeticOwnership grant = temporary[i];
                string warnKey = grant.Id.Value + "@" + grant.ExpiresAtSeconds.ToString("F0");
                double remaining = grant.RemainingSeconds(now);

                if (remaining > expiringSoonThresholdSeconds || remaining <= 0d)
                {
                    continue;
                }

                if (!_warnedForExpiry.Add(warnKey))
                {
                    continue;
                }

                ProgressionNotificationBridge.Raise(NotificationType.TemporaryItemExpiringSoon, grant.Id.Value + " expires soon!");
            }
        }
    }
}
