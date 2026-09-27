using System.Collections.Generic;
using UnityEngine;

namespace TheStrangerTheyAre
{
    public class VeryActiveMeteorLauncher : MeteorLauncher, ILateInitializer
    {
        public int meteorCount = 256;
        public int dynamicMeteorCount => meteorCount / 2;

        private readonly Dictionary<MeteorController, float> _forcedSuspendDelays = new();

        public new void Awake()
        {
            _parentBody = gameObject.GetAttachedOWRigidbody();
            _initialized = false;
            LateInitializerManager.RegisterLateInitializer(this);
        }

        public new void OnDestroy()
        {
            if (!_initialized)
            {
                LateInitializerManager.UnregisterLateInitializer(this);
            }
        }

        public new void LateInitialize()
        {
            _initialized = true;

            if (_meteorPrefab != null)
            {
                _meteorPool = new List<MeteorController>(meteorCount);
                _launchedMeteors = new List<MeteorController>(meteorCount);

                for (int i = 0; i < meteorCount; i++)
                {
                    MeteorController meteor = Instantiate(_meteorPrefab).GetRequiredComponent<MeteorController>();
                    meteor.Suspend(transform);
                    _meteorPool.Add(meteor);
                }
            }

            if (_dynamicMeteorPrefab != null)
            {
                _dynamicMeteorPool = new List<MeteorController>(dynamicMeteorCount);
                _launchedDynamicMeteors = new List<MeteorController>(dynamicMeteorCount);

                for (int j = 0; j < dynamicMeteorCount; j++)
                {
                    MeteorController dynamicMeteor = Instantiate(_dynamicMeteorPrefab).GetRequiredComponent<MeteorController>();
                    dynamicMeteor.Suspend(transform);
                    _dynamicMeteorPool.Add(dynamicMeteor);
                }
            }
        }

        public new void FixedUpdate()
        {
            RecycleSuspended(_launchedMeteors, _meteorPool);
            RecycleSuspended(_launchedDynamicMeteors, _dynamicMeteorPool);

            if (!_initialized || Time.time <= _lastLaunchTime + _launchDelay)
                return;

            if (!_areParticlesPlaying)
            {
                _areParticlesPlaying = true;

                foreach (var particle in _launchParticles)
                {
                    particle.Play();
                }
            }

            if (Time.time <= _lastLaunchTime + _launchDelay + 2.3f)
                return;

            // If we're out, this will explode the oldest meteor.
            // Don't reset the timer so we'll retry as soon as it suspends.
            if (!LaunchMeteor())
                return;

            _lastLaunchTime = Time.time;
            _launchDelay = Random.Range(_minInterval, _maxInterval);
            _areParticlesPlaying = false;

            foreach (var particle in _launchParticles)
            {
                particle.Stop();
            }
        }

        private void RecycleSuspended(
           List<MeteorController> launched,
           List<MeteorController> pool)
        {
            if (launched == null)
                return;

            for (int i = launched.Count - 1; i >= 0; i--)
            {
                var meteor = launched[i];

                if (meteor == null)
                {
                    launched.QuickRemoveAt(i);
                    continue;
                }

                if (!meteor.isSuspended)
                    continue;

                // Restore its normal impact suspend delay after a forced explosion.
                if (_forcedSuspendDelays.TryGetValue(meteor, out float oldDelay))
                {
                    meteor._impactSuspendDelay = oldDelay;
                    _forcedSuspendDelays.Remove(meteor);
                }

                // Suspended meteors are no longer considered launched.
                launched.QuickRemoveAt(i);
                pool.Add(meteor);
            }
        }

        private new bool LaunchMeteor()
        {
            bool dynamic =
                _dynamicMeteorPool != null &&
                (_meteorPool == null || Random.value < _dynamicProbability);

            MeteorController meteor = null;

            if (dynamic)
            {
                if (_dynamicMeteorPool.Count == 0)
                {
                    ExplodeOldest(_launchedDynamicMeteors);
                    return false;
                }

                meteor = _dynamicMeteorPool[_dynamicMeteorPool.Count - 1];
                meteor.Initialize(transform, null, null);

                _dynamicMeteorPool.QuickRemoveAt(_dynamicMeteorPool.Count - 1);
                _launchedDynamicMeteors.Add(meteor);
            }
            else
            {
                if (_meteorPool.Count == 0)
                {
                    ExplodeOldest(_launchedMeteors);
                    return false;
                }

                meteor = _meteorPool[_meteorPool.Count - 1];
                meteor.Initialize(transform, _detectableField, _detectableFluid);

                _meteorPool.QuickRemoveAt(_meteorPool.Count - 1);
                _launchedMeteors.Add(meteor);
            }

            Vector3 linearVelocity =
                _parentBody.GetPointVelocity(transform.position) +
                transform.TransformDirection(_launchDirection) *
                Random.Range(_minLaunchSpeed, _maxLaunchSpeed);

            Vector3 angularVelocity = transform.forward * 2f;

            meteor.Launch(
                null,
                transform.position,
                transform.rotation,
                linearVelocity,
                angularVelocity
            );

            if (_audioSector.ContainsOccupant(DynamicOccupant.Player))
            {
                _launchSource.pitch = Random.Range(0.4f, 0.6f);
                _launchSource.PlayOneShot(AudioType.BH_MeteorLaunch, 1f);
            }

            return true;
        }

        private void ExplodeOldest(List<MeteorController> launched)
        {
            if (launched == null || launched.Count == 0)
                return;

            MeteorController oldest = null;
            float oldestLaunchTime = float.MaxValue;

            foreach (var meteor in launched)
            {
                if (meteor == null || meteor.isSuspended)
                    continue;

                if (meteor._launchTime < oldestLaunchTime)
                {
                    oldest = meteor;
                    oldestLaunchTime = meteor._launchTime;
                }
            }

            if (oldest == null)
                return;

            // Already exploded, just make it return to the pool immediately.
            if (oldest.hasImpacted)
            {
                ForceQuickSuspend(oldest);
                return;
            }

            ForceQuickSuspend(oldest);

            // Force it through the normal meteor impact/explosion effects.
            oldest.Impact(
                gameObject,
                oldest.transform.position,
                oldest.owRigidbody.GetVelocity()
            );
        }

        private void ForceQuickSuspend(MeteorController meteor)
        {
            if (!_forcedSuspendDelays.ContainsKey(meteor))
            {
                _forcedSuspendDelays.Add(meteor, meteor._impactSuspendDelay);
            }

            meteor._impactSuspendDelay = 0f;
        }
    }
}