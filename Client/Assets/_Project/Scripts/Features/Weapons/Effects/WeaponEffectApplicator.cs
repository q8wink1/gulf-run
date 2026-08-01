using System.Collections;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Pooling;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Weapons.Configuration;
using UnityEngine;

namespace GulfRun.Features.Weapons.Effects
{
    /// <summary>
    /// Reacts to every confirmed <see cref="WeaponHitEvent"/>: resolves the
    /// actual <see cref="PlayerStatusEffect"/> (applying the Falcon Feather
    /// Mark bonus if the target was marked), applies it to the target via
    /// <see cref="PlayerStatusEffectRegistry"/> if a live receiver exists for
    /// that connection, and plays the weapon's impact feedback (sound +
    /// pooled particle — never Instantiate/Destroy, per the Performance
    /// requirement). Scene-scoped like <c>RemotePlayerSyncHub</c>/
    /// <c>SpawnManager</c> since it only matters during actual gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WeaponEffectApplicator : SceneSingleton<WeaponEffectApplicator>
    {
        [SerializeField] private WeaponCatalogConfig catalog;
        [SerializeField] private float impactParticleLifetimeSeconds = 1.5f;

        private IMatchTransport _transport;

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            _transport.WeaponHitConfirmed += HandleWeaponHitConfirmed;
        }

        private void OnDisable()
        {
            if (_transport != null)
            {
                _transport.WeaponHitConfirmed -= HandleWeaponHitConfirmed;
            }
        }

        private void HandleWeaponHitConfirmed(WeaponHitEvent hit)
        {
            WeaponDefinition definition = catalog != null ? catalog.GetDefinition(hit.Weapon) : null;
            if (definition == null)
            {
                return;
            }

            bool blocked = false;
            if (PlayerStatusEffectRegistry.TryGet(hit.TargetConnectionId, out IPlayerStatusEffectReceiver receiver))
            {
                // The Mark bonus applies to THIS hit (the "next successful
                // attack against that opponent"), then is consumed — so read
                // it before applying, and clear it regardless of outcome.
                bool targetWasMarked = receiver.IsMarked;
                PlayerStatusEffect effect = WeaponEffectResolver.Resolve(hit.Weapon, definition.EffectFlags, definition.Magnitude, definition.DurationSeconds, targetWasMarked);
                blocked = !receiver.TryApplyEffect(effect);

                if (targetWasMarked)
                {
                    receiver.ClearMark();
                }
            }
            // else: no live receiver for this connection yet (no networked
            // remote avatar exists today — Sprint 4/5 remaining TODO). Still
            // play feedback below so activation is never silent.

            PlayImpactFeedback(definition, blocked);
        }

        private void PlayImpactFeedback(WeaponDefinition definition, bool blocked)
        {
            AudioManager.Instance?.PlayOneShot(blocked ? definition.CooldownSound : definition.ImpactSound);

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
