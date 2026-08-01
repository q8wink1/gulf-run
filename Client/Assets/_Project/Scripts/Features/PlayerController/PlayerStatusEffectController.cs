using System.Collections.Generic;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.PlayerController
{
    /// <summary>
    /// Applies confirmed Sprint 5 weapon effects to this player's local
    /// simulation and registers itself (by connection id) into
    /// <see cref="PlayerStatusEffectRegistry"/> so Features.Weapons can find
    /// it without this feature ever referencing Features.Weapons back —
    /// the same decoupling pattern as <see cref="PlayerNetworkStateAdapter"/>.
    ///
    /// Every effect here is temporary and reversible (auto-expires), so a
    /// weapon can never permanently eliminate a player, per the Sprint 5
    /// brief. Not wired into <c>Player.prefab</c> yet — same "no Player
    /// prefab instance dropped into Gameplay.unity" decision Sprints 2-4
    /// documented; this component is ready to attach the moment that changes.
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    public sealed class PlayerStatusEffectController : MonoBehaviour, IPlayerStatusEffectReceiver
    {
        private readonly List<ActiveEffect> _active = new List<ActiveEffect>();
        private PlayerMotor _motor;
        private int _connectionId = -1;

        public bool IsMarked { get; private set; }

        private void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
        }

        private void OnEnable()
        {
            _connectionId = MatchTransportService.Current != null ? MatchTransportService.Current.LocalConnectionId : -1;
            PlayerStatusEffectRegistry.Register(_connectionId, this);
        }

        private void OnDisable()
        {
            PlayerStatusEffectRegistry.Unregister(_connectionId, this);
        }

        private void Update()
        {
            if (_active.Count == 0)
            {
                return;
            }

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                ActiveEffect effect = _active[i];
                effect.RemainingSeconds -= Time.deltaTime;
                if (effect.RemainingSeconds <= 0f)
                {
                    _active.RemoveAt(i);
                }
                else
                {
                    _active[i] = effect;
                }
            }

            Recompute();
        }

        public bool TryApplyEffect(PlayerStatusEffect effect)
        {
            if (effect.Has(WeaponEffectFlags.Shield))
            {
                _active.Add(new ActiveEffect(WeaponEffectFlags.Shield, float.MaxValue));
                Recompute();
                return true;
            }

            if (ConsumeShieldIfPresent())
            {
                // A shield absorbs exactly one incoming negative effect, then is gone.
                return false;
            }

            if (effect.Has(WeaponEffectFlags.Cleanse))
            {
                ClearNegativeEffects();
            }

            if (effect.Has(WeaponEffectFlags.Mark))
            {
                IsMarked = true;
            }

            WeaponEffectFlags movementFlags = effect.Flags & ~(WeaponEffectFlags.Cleanse | WeaponEffectFlags.Mark | WeaponEffectFlags.Shield);
            if (movementFlags != WeaponEffectFlags.None)
            {
                _active.Add(new ActiveEffect(movementFlags, (float)effect.DurationSeconds, effect.Magnitude));
            }

            if (effect.Has(WeaponEffectFlags.Mark))
            {
                // Marking is a status on the target for the *next attacker*
                // to benefit from (see WeaponEffectResolver) — being marked
                // is consumed the next time a hit is confirmed against us.
            }

            Recompute();
            return true;
        }

        /// <summary>Called by Features.Weapons.Authority once this player successfully lands a hit on a marked opponent — clears the mark so the bonus applies once.</summary>
        public void ClearMark() => IsMarked = false;

        private bool ConsumeShieldIfPresent()
        {
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].Flags == WeaponEffectFlags.Shield)
                {
                    _active.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void ClearNegativeEffects()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                WeaponEffectFlags flags = _active[i].Flags;
                bool isNegative = (flags & (WeaponEffectFlags.Slow | WeaponEffectFlags.VisionReduced | WeaponEffectFlags.Pause |
                                             WeaponEffectFlags.Stun | WeaponEffectFlags.TractionLoss | WeaponEffectFlags.Knockdown)) != 0;
                if (isNegative)
                {
                    _active.RemoveAt(i);
                }
            }
        }

        private void Recompute()
        {
            float speedMultiplier = 1f;
            bool locked = false;

            for (int i = 0; i < _active.Count; i++)
            {
                ActiveEffect effect = _active[i];

                if (effect.Has(WeaponEffectFlags.Stun) || effect.Has(WeaponEffectFlags.Pause) || effect.Has(WeaponEffectFlags.Knockdown))
                {
                    locked = true;
                }

                if (effect.Has(WeaponEffectFlags.Slow) || effect.Has(WeaponEffectFlags.TractionLoss))
                {
                    speedMultiplier *= Mathf.Clamp(effect.Magnitude, 0.05f, 1f);
                }

                if (effect.Has(WeaponEffectFlags.SpeedBoost))
                {
                    speedMultiplier *= Mathf.Max(effect.Magnitude, 1f);
                }
            }

            _motor.SetMovementLocked(locked);
            _motor.SetExternalSpeedMultiplier(speedMultiplier);
        }

        private struct ActiveEffect
        {
            public WeaponEffectFlags Flags;
            public float RemainingSeconds;
            public float Magnitude;

            public ActiveEffect(WeaponEffectFlags flags, float remainingSeconds, float magnitude = 1f)
            {
                Flags = flags;
                RemainingSeconds = remainingSeconds;
                Magnitude = magnitude;
            }

            public bool Has(WeaponEffectFlags flag) => (Flags & flag) != 0;
        }
    }
}
