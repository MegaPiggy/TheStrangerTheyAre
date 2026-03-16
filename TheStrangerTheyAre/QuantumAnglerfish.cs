using System.Linq;
using UnityEngine;

namespace TheStrangerTheyAre
{
    // TODO: Finish this
    public class QuantumAnglerfish : QuantumObject
    {
        [SerializeField]
        private float _noAnglerfishRadius = 100f; // Safe radius in meters
        [SerializeField]
        private SphereShape _sectorShape;
        [SerializeField]
        private VisibilityTracker _visibilityTracker;
        [SerializeField]
        private float _sphereCheckRadius = 2f;

        public override void Awake()
        {
            base.Awake();
            if (_sectorShape == null)
            {
                TheStrangerTheyAre.WriteLine("QuantumAnglerfish has no sector shape assigned. It will attempt to find one at runtime, but this may cause issues if the bramble sphere isn't found.", OWML.Common.MessageType.Warning);
                _sectorShape = GetComponentInParent<SphereShape>();
            }
            if (_visibilityTracker == null)
            {
                TheStrangerTheyAre.WriteLine("QuantumAnglerfish has no visibility tracker assigned. It will attempt to find one at runtime, but this may cause issues if the visibility tracker isn't found.", OWML.Common.MessageType.Warning);
                _visibilityTracker = GetComponentInChildren<VisibilityTracker>();
            }
            _maxSnapshotLockRange = 1000; // amplifies snapshot range to fit the anglerfish, so snapshots can be made
        }

        public override bool ChangeQuantumState(bool skipInstantVisibilityCheck)
        {
            // Try several times to find a valid, non-occluded position on the bramble sphere
            SphereShape sphere = GetCurrentBrambleSphere();
            if (sphere == null)
            {
                TheStrangerTheyAre.WriteLine("QuantumAnglerfish couldn't find a valid SphereShape to reposition within.", OWML.Common.MessageType.Warning);
                return false;
            }

            Transform sphereTransform = sphere.transform;

            for (int i = 1; i <= 10; i++)
            {
                // Get random point on sphere
                Vector3 localPoint = UnityEngine.Random.onUnitSphere * sphere.radius;
                Vector3 globalPoint = sphereTransform.TransformPoint(localPoint);

                // Keep a minimum distance from the player
                Vector3 playerPos = Locator.GetPlayerTransform().position;
                Vector3 offsetFromPlayer = globalPoint - playerPos;
                if (offsetFromPlayer.magnitude < _noAnglerfishRadius)
                {
                    offsetFromPlayer = offsetFromPlayer.normalized * _noAnglerfishRadius;
                    globalPoint = playerPos + offsetFromPlayer;
                }

                // Quick occupancy check so we don't place the anglerfish inside geometry
                Collider[] hits = Physics.OverlapSphere(globalPoint, _sphereCheckRadius);
                if (hits != null && hits.Length > 0)
                {
                    TheStrangerTheyAre.WriteLine($"QuantumAnglerfish candidate position {globalPoint} is occupied by {hits.Length} colliders [{string.Join(", ", hits.Select(c => c.name))}], trying again.", OWML.Common.MessageType.Debug);
                    // occupied, try again
                    continue;
                }

                // Position the visibility tracker first so visibility checks happen at the candidate position
                if (_visibilityTracker != null)
                {
                    _visibilityTracker.transform.position = globalPoint;
                    if (!Physics.autoSyncTransforms) Physics.SyncTransforms();
                }

                // If we are skipping instant visibility or the object isn't instantly visible, commit the position
                if (skipInstantVisibilityCheck || !CheckVisibilityInstantly())
                {
                    TheStrangerTheyAre.WriteLine($"QuantumAnglerfish repositioned to {globalPoint} after {i} attempts.", OWML.Common.MessageType.Info);

                    Quaternion randomYRot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                    OWRigidbody rb = GetComponent<OWRigidbody>();
                    if (rb != null)
                    {
                        rb.SetPosition(globalPoint);
                        rb.SetRotation(randomYRot);
                    }
                    else
                    {
                        transform.position = globalPoint;
                        transform.rotation = randomYRot;
                    }

                    if (!Physics.autoSyncTransforms) Physics.SyncTransforms();

                    if (_visibilityTracker != null)
                    {
                        _visibilityTracker.transform.localPosition = Vector3.zero;
                    }

                    return true;
                }

                // Reset visibility tracker local offset and try again
                if (_visibilityTracker != null)
                {
                    _visibilityTracker.transform.localPosition = Vector3.zero;
                }
            }

            return false;
        }

        // TODO: Replace this with a reference to a sphere shape from the dimension 
        private SphereShape GetCurrentBrambleSphere()
        {
            return _sectorShape;
        }
    }
}