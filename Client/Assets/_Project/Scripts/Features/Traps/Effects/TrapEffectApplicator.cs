using System.Collections;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Pooling;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Traps.Configuration;
using UnityEngine;

namespace GulfRun.Features.Traps.Effects
{
    /// <summary>
    /// Reacts to every confirmed <see cref="TrapTriggerEvent"/>: builds the
    /// <see cref="PlayerStatusEffect"/> straight from the triggered trap's
    /// <see cref="TrapDefinition"/> (no "Marked" bonus concept like weapons
    /// have — traps have no attacker to reward), applies it to the target
    /// via <see cref="PlayerStatusEffectRegistry"/> if a live receiver exists
    /// for that connection, and plays the trap's trigger feedback (sound +
    /// pooled particle — never Instantiate/Destroy). Exactly the same role
    /// <c>Features.Weapons.Effects.WeaponEffectApplicator</c> plays for
    /// weapons, reusing the identical effect-application pipeline.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TrapEffectApplicator : SceneSingleton<TrapEffectApplicator>
    {
        [SerializeField] private TrapCatalogConfig catalog;
        [SerializeField] private float impactParticleLifetimeSeconds = 1.5f;

        private IMatchTransport _transport;

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.TrapTriggerConfirmed += HandleTrapTriggerConfirmed;
        }

        private void OnDisable()
        {
            if (_transport != null)
            {
                _transport.TrapTriggerConfirmed -= HandleTrapTriggerConfirmed;
            }
        }

        private void HandleTrapTriggerConfirmed(TrapTriggerEvent hit)
        {
            TrapDefinition definition = catalog != null ? catalog.GetDefinition(hit.Trap) : null;
            if (definition == null)
            {
                return;
            }

            if (PlayerStatusEffectRegistry.TryGet(hit.TargetConnectionId, out IPlayerStatusEffectReceiver receiver))
            {
                var effect = new PlayerStatusEffect(definition.EffectFlags, definition.DurationSeconds, definition.Magnitude, EffectSourceKind.Trap, (int)hit.Trap);
                receiver.TryApplyEffect(effect);
            }
            // else: no live receiver for this connection yet (no networked
            // remote avatar exists today — Sprint 4/5/6 remaining TODO).
            // Still play feedback below so a trigger is never silent.

            // Sprint 9 "Player Statistics: Traps Hit" hook — only for a hit
            // against the local player, never a remote participant's.
            if (_transport != null && hit.TargetConnectionId == _transport.LocalConnectionId)
            {
                PlayerStatEventService.RaiseLocalTrapHit();
            }

            PlayImpactFeedback(definition);
        }

        private void PlayImpactFeedback(TrapDefinition definition)
        {
            AudioManager.Instance?.PlayOneShot(definition.TriggerSound);

            if (definition.ImpactParticlePrefab == null || ObjectPoolManager.Instance == null)
            {
                return;
            }

            GameObject fx = ObjectPoolManager.Instance.Get(definition.ImpactParticlePrefab, transform.position, Quaternion.identity, transform);
            if (fx != null)
            {
                StartCoroutine(ReleaseAfterDelay(fx, impactParticleLifetimeSeconds));
            }
        }

        private IEnumerator ReleaseAfterDelay(GameObject instance, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            ObjectPoolManager.Instance?.Release(instance);
        }
    }
}
