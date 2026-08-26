using System.Collections.Generic;
using UnityEngine;

namespace TheStrangerTheyAre
{
    public class VeryActiveMeteorLauncher : MeteorLauncher, ILateInitializer
    {
        public int meteorCount = 256;
        public int dynamicMeteorCount => meteorCount / 2;

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
    }
}