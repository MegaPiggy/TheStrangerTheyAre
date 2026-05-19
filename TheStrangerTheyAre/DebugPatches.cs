using HarmonyLib;
using UnityEngine;
using NewHorizons.Utility;
using OWML.Common;

namespace TheStrangerTheyAre;

#if DEBUG
[HarmonyPatch]
public class DebugPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GhostAirlock), nameof(GhostAirlock.OnClose))]
    public static bool GhostAirlock_OnClose_Patch(GhostAirlock __instance)
    {
        if (__instance._outerOneShotAudio != null)
        {
            __instance._outerOneShotAudio.PlayOneShot(AudioType.Airlock_Open, 1f);
        }
        __instance._bothDoorsClosed = false;
        if (__instance._outerDoor != null)
        {
            __instance._outerDoor.Open();
        }
        else
        {
            TheStrangerTheyAre.WriteLine("Outer door is null at " + __instance.transform.GetPath(), MessageType.Error);
        }
        if (__instance._airlockVolume != null)
        {
            if (__instance._interiorSectorVolume != null)
            {
                for (int i = 0; i < __instance._airlockVolume.getTrackedObjects().Count; i++)
                {
                    __instance._interiorSectorVolume.RemoveObjectFromVolume(__instance._airlockVolume.getTrackedObjects()[i]);
                }
            }
            else
            {
                TheStrangerTheyAre.WriteLine("Interior sector volume is null at " + __instance.transform.GetPath(), MessageType.Error);
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine("Airlock volume is null at " + __instance.transform.GetPath(), MessageType.Error);
        }
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GhostAirlock), nameof(GhostAirlock.OnOuterDoorFinishClosing))]
    public static bool GhostAirlock_OnOuterDoorFinishClosing_Patch(GhostAirlock __instance)
    {
        if (__instance._airlockVolume != null)
        {
            if (__instance._interiorSectorVolume != null)
            {
                for (int i = 0; i < __instance._airlockVolume.getTrackedObjects().Count; i++)
                {
                    __instance._interiorSectorVolume.AddObjectToVolume(__instance._airlockVolume.getTrackedObjects()[i]);
                }
            }
            else
            {
                TheStrangerTheyAre.WriteLine("Interior sector volume is null at " + __instance.transform.GetPath(), MessageType.Error);
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine("Airlock volume is null at " + __instance.transform.GetPath(), MessageType.Error);
        }
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(OWLightController), nameof(OWLightController.UpdateVisuals))]
    public static void OWLightController_UpdateVisuals_Patch(OWLightController __instance)
    {
        if (__instance._lights != null)
        {
            for (int i = 0; i < __instance._lights.Length; i++)
            {
                if (__instance._lights[i] == null)
                {
                    TheStrangerTheyAre.WriteLine("Light at index " + i + " is null at " + __instance.transform.GetPath(), MessageType.Error);
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine("Lights are null at " + __instance.transform.GetPath(), MessageType.Error);
        }
        if (__instance._renderers != null)
        {
            for (int i = 0; i < __instance._renderers.Length; i++)
            {
                if (__instance._renderers[i] == null)
                {
                    TheStrangerTheyAre.WriteLine("Renderer at index " + i + " is null at " + __instance.transform.GetPath(), MessageType.Error);
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine("Renderers are null at " + __instance.transform.GetPath(), MessageType.Error);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DreamLanternController), nameof(DreamLanternController.Awake))]
    public static void DreamLanternController_Awake_Patch(DreamLanternController __instance)
    {
        // Lens flare sanity
        if (__instance._lensFlare == null)
        {
            TheStrangerTheyAre.WriteLine(
                "DreamLanternController._lensFlare is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Petals sanity
        if (__instance._focuserPetals != null)
        {
            for (int i = 0; i < __instance._focuserPetals.Length; i++)
            {
                if (__instance._focuserPetals[i] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "DreamLanternController._focuserPetals[" + i + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "DreamLanternController._focuserPetals is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Roots sanity
        if (__instance._concealerRoots != null)
        {
            for (int i = 0; i < __instance._concealerRoots.Length; i++)
            {
                if (__instance._concealerRoots[i] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "DreamLanternController._concealerRoots[" + i + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "DreamLanternController._concealerRoots is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Covers sanity
        if (__instance._concealerCovers != null)
        {
            for (int i = 0; i < __instance._concealerCovers.Length; i++)
            {
                if (__instance._concealerCovers[i] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "DreamLanternController._concealerCovers[" + i + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "DreamLanternController._concealerCovers is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Optional: component existence sanity (since Awake grabs it later)
        // This won't call GetRequiredComponentInChildren (which might throw); it just checks presence.
        if (__instance.GetComponentInChildren<LightSourceVolume>(true) == null)
        {
            TheStrangerTheyAre.WriteLine(
                "DreamLanternController missing LightSourceVolume in children at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CullGroup), nameof(CullGroup.Awake))]
    public static void CullGroup_Awake_Patch(CullGroup __instance)
    {
        if (__instance._waitForStreaming && __instance._streamingMeshes != null)
        {
            for (int i = 0; i < __instance._streamingMeshes.Count; i++)
            {
                if (__instance._streamingMeshes[i] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "CullGroup._streamingMeshes[" + i + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                    break; // No need to spam if one is null, likely all are
                }
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EclipseDoorController), nameof(EclipseDoorController.Awake))]
    public static void EclipseDoorController_Awake_Patch(EclipseDoorController __instance)
    {
        // Rotating elements check
        if (__instance._rotatingElements != null)
        {
            if (__instance._rotatingElements.Length < 1)
            {
                TheStrangerTheyAre.WriteLine(
                    "EclipseDoorController._rotatingElements is empty at " + __instance.transform.GetPath(),
                    MessageType.Error
                );
            }
            else
            {
                for (int i = 0; i < __instance._rotatingElements.Length; i++)
                {
                    if (__instance._rotatingElements[i] == null)
                    {
                        TheStrangerTheyAre.WriteLine(
                            "EclipseDoorController._rotatingElements[" + i + "] is null at " + __instance.transform.GetPath(),
                            MessageType.Error
                        );
                    }
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "EclipseDoorController._rotatingElements is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Light sensors check
        if (__instance._lightSensors != null)
        {
            if (__instance._lightSensors.Length < 1)
            {
                TheStrangerTheyAre.WriteLine(
                    "EclipseDoorController._lightSensors is empty at " + __instance.transform.GetPath(),
                    MessageType.Error
                );
            }
            else
            {
                for (int i = 0; i < __instance._lightSensors.Length; i++)
                {
                    if (__instance._lightSensors[i] == null)
                    {
                        TheStrangerTheyAre.WriteLine(
                            "EclipseDoorController._lightSensors[" + i + "] is null at " + __instance.transform.GetPath(),
                            MessageType.Error
                        );
                    }
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "EclipseDoorController._lightSensors is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Front door check (only relevant if sensors are disabled while open)
        if (__instance._disableSensorsWhileOpen)
        {
            if (__instance._frontDoor == null)
            {
                TheStrangerTheyAre.WriteLine(
                    "EclipseDoorController._frontDoor is null while _disableSensorsWhileOpen is true at " + __instance.transform.GetPath(),
                    MessageType.Error
                );
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DetachableFragmentColliderSwapper), nameof(DetachableFragmentColliderSwapper.Awake))]
    public static void DetachableFragmentColliderSwapper_Awake_Patch(DetachableFragmentColliderSwapper __instance)
    {
        // Check if DetachableFragment exists before Awake tries to fetch it
        if (__instance.GetComponent<DetachableFragment>() == null)
        {
            TheStrangerTheyAre.WriteLine(
                "DetachableFragmentColliderSwapper missing DetachableFragment at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(QuadUVMapper), nameof(QuadUVMapper.GenerateMesh))]
    public static void QuadUVMapper_GenerateMesh_Patch(QuadUVMapper __instance)
    {
        if (__instance.GetComponent<MeshFilter>() == null)
        {
            TheStrangerTheyAre.WriteLine(
                "QuadUVMapper missing MeshFilter at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(DreamObjectCandleProjector), nameof(DreamObjectCandleProjector.Start))]
    public static void DreamObjectCandleProjector_Start_Patch(DreamObjectCandleProjector __instance)
    {
        // Controlling candles (front)
        if (__instance._controllingCandles != null)
        {
            for (int i = 0; i < __instance._controllingCandles.Length; i++)
            {
                if (__instance._controllingCandles[i] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "DreamObjectCandleProjector._controllingCandles[" + i + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "DreamObjectCandleProjector._controllingCandles is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Back controlling candles
        if (__instance._controllingCandlesBack != null)
        {
            for (int i = 0; i < __instance._controllingCandlesBack.Length; i++)
            {
                if (__instance._controllingCandlesBack[i] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "DreamObjectCandleProjector._controllingCandlesBack[" + i + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "DreamObjectCandleProjector._controllingCandlesBack is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }

        // Projections array + elements
        if (__instance._projections != null)
        {
            for (int j = 0; j < __instance._projections.Length; j++)
            {
                if (__instance._projections[j] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "DreamObjectCandleProjector._projections[" + j + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "DreamObjectCandleProjector._projections is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MindProjectorTrigger), "UpdateParticlesState")]
    public static void MindProjectorTrigger_UpdateParticlesState_Patch(MindProjectorTrigger __instance)
    {
        // Particles array check
        if (__instance._particles != null)
        {
            for (int i = 0; i < __instance._particles.Length; i++)
            {
                if (__instance._particles[i] == null)
                {
                    TheStrangerTheyAre.WriteLine(
                        "MindProjectorTrigger._particles[" + i + "] is null at " + __instance.transform.GetPath(),
                        MessageType.Error
                    );
                }
            }
        }
        else
        {
            TheStrangerTheyAre.WriteLine(
                "MindProjectorTrigger._particles is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(SingleLightSensor), "OnSectorOccupantsUpdated")]
    public static void SingleLightSensor_OnSectorOccupantsUpdated_Patch(SingleLightSensor __instance)
    {
        // Light detector existence
        if (__instance._lightDetector == null)
        {
            TheStrangerTheyAre.WriteLine(
                "SingleLightSensor._lightDetector is null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
            return;
        }

        // Shape existence (GetShape() could return null)
        var shape = __instance._lightDetector.GetShape();
        if (shape == null)
        {
            TheStrangerTheyAre.WriteLine(
                "SingleLightSensor._lightDetector.GetShape() returned null at " + __instance.transform.GetPath(),
                MessageType.Error
            );
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AudioVolume), nameof(AudioVolume.Deactivate))]
    public static void AudioVolume_Deactivate_Patch(AudioVolume __instance, float fadeSeconds)
    {
        TheStrangerTheyAre.WriteLine($"AudioVolume.Deactivate called on instance of type: {__instance.GetType().FullName}", MessageType.Error);
    }
}
#endif