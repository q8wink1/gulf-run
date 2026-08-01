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
    public sealed class PlayerStatusEffectController : MonoBehaviour, IPlayerStatusEffectReceiver, IActiveEffectsHudProvider
    {
        private readonly List<ActiveEffect> _active = new List<ActiveEffect>();
        private readonly List<ActiveHudEffectSnapshot> _hudEffects = new List<ActiveHudEffectSnapshot>(8);
        private PlayerMotor _motor;
        private int _connectionId = -1;

        public bool IsMarked { get; private set; }

        bool IActiveEffectsHudProvider.HasShield
        {
            get
            {
                for (int i = 0; i < _active.Count; i++)
                {
                    if (_active[i].Has(WeaponEffectFlags.Shield))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        bool IActiveEffectsHudProvider.HasSpeedBoost
        {
            get
            {
                for (int i = 0; i < _active.Count; i++)
                {
                    if (_active[i].Has(WeaponEffectFlags.SpeedBoost))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        IReadOnlyList<ActiveHudEffectSnapshot> IActiveEffectsHudProvider.ActiveEffects
        {
            get
            {
                RebuildHudEffects();
                return _hudEffects;
            }
        }

        private void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
        }

        private void OnEnable()
        {
            _connectionId = MatchTransportService.Current != null ? MatchTransportService.Current.LocalConnectionId : -1;
            PlayerStatusEffectRegistry.Register(_connectionId, this);
            ActiveEffectsHudService.Current = this;
        }

        private void OnDisable()
        {
            PlayerStatusEffectRegistry.Unregister(_connectionId, this);
            if (ReferenceEquals(ActiveEffectsHudService.Current, this))
            {
                ActiveEffectsHudService.Current = null;
            }
        }

        private void RebuildHudEffects()
        {
            _hudEffects.Clear();
            for (int i = 0; i < _active.Count; i++)
            {
                ActiveEffect effect = _active[i];
                if (!HudEffectKindResolver.TryResolve(effect.Flags, out HudEffectKind kind))
                {
                    continue;
                }

                float total = effect.TotalSeconds > 0f && effect.TotalSeconds < float.MaxValue * 0.5f
                    ? effect.TotalSeconds
                    : Mathf.Max(effect.RemainingSeconds, 1f);
                _hudEffects.Add(new ActiveHudEffectSnapshot(kind, effect.RemainingSeconds, total));
            }
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
                _active.Add(new ActiveEffect(WeaponEffectFlags.Shield, float.MaxValue, 1f, float.MaxValue));
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

            if (effect.Has(WeaponEffectFlags.LateralPush))
            {
                // One-shot, not duration-based — apply immediately and never
                // add it to _active (there is nothing to Recompute() every
                // frame for an instantaneous push; see PlayerMotor.ApplyLateralImpulse).
                _motor.ApplyLateralImpulse(effect.Magnitude);
            }

            WeaponEffectFlags movementFlags = effect.Flags & ~(WeaponEffectFlags.Cleanse | WeaponEffectFlags.Mark | WeaponEffectFlags.Shield | WeaponEffectFlags.LateralPush);
            if (movementFlags != WeaponEffectFlags.None)
            {
                float duration = (float)effect.DurationSeconds;
                _active.Add(new ActiveEffect(movementFlags, duration, effect.Magnitude, duration));
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
            public float TotalSeconds;

            public ActiveEffect(WeaponEffectFlags flags, float remainingSeconds, float magnitude = 1f, float totalSeconds = 0f)
            {
                Flags = flags;
                RemainingSeconds = remainingSeconds;
                Magnitude = magnitude;
                TotalSeconds = totalSeconds > 0f ? totalSeconds : remainingSeconds;
            }

            public bool Has(WeaponEffectFlags flag) => (Flags & flag) != 0;
        }
    }
}
