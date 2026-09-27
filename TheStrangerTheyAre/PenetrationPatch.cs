using HarmonyLib;
using OWML.Common;
using UnityEngine;

namespace TheStrangerTheyAre
{
    [HarmonyPatch]
    public static class PenetrationPatch
    {
        public static string GetPath(this Transform current)
        {
            if (current.parent == null) return current.name;
            return current.parent.GetPath() + "/" + current.name;
        }

        public static float GetDistToSurface(BoxCollider boxCollider, Vector3 worldPosition)
        {
            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);

            Vector3 scale = boxCollider.transform.lossyScale;
            Vector3 size = Vector3.Scale(boxCollider.size, new Vector3(
                Mathf.Abs(scale.x),
                Mathf.Abs(scale.y),
                Mathf.Abs(scale.z)
            ));

            Vector3 offset = worldPosition - center;
            Vector3 extents = size * 0.5f;

            Vector3 penetration = extents;

            penetration.x -= Mathf.Abs(Vector3.Dot(offset, boxCollider.transform.right));
            penetration.y -= Mathf.Abs(Vector3.Dot(offset, boxCollider.transform.up));
            penetration.z -= Mathf.Abs(Vector3.Dot(offset, boxCollider.transform.forward));

            int axis = 0;
            float closest = Mathf.Abs(penetration.x);

            for (int i = 1; i < 3; i++)
            {
                float distance = Mathf.Abs(penetration[i]);
                if (distance < closest)
                {
                    axis = i;
                    closest = distance;
                }
            }

            return penetration[axis];
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(OWTriggerVolume), nameof(OWTriggerVolume.GetPenetrationDistance))]
        public static bool OWTriggerVolume_GetPenetrationDistance_Patch(OWTriggerVolume __instance, Vector3 worldPos, ref float __result)
        {
            if (__instance._childEntryways.Count > 0 || __instance._sharedEntryways.Length != 0)
            {
                return true;
            }
            else if (__instance._owCollider != null)
            {
                Collider collider = __instance._owCollider.GetCollider();

                if (collider is BoxCollider box)
                {
                    __result = GetDistToSurface(box, worldPos);
                    return false;
                }
                else if (collider is SphereCollider or CapsuleCollider)
                {
                    return true;
                }
                else
                {
                    TheStrangerTheyAre.WriteLine(
                        $"GetPenetrationDistance: OWTriggerVolume={__instance.transform.GetPath()}, " +
                        $"OWCollider={__instance._owCollider.GetType().Name}, " +
                        $"Collider={(collider != null ? collider.GetType().Name : "null")}",
                        MessageType.Error
                    );
                    return false;
                }
            }

            return true;
        }
    }
}
