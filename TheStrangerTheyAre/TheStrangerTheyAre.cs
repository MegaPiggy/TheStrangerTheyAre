using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TheStrangerTheyAre
{
    public class TheStrangerTheyAre : ModBehaviour
    {
        public static INewHorizons NewHorizonsAPI { get; private set; }
        private AssetBundle endingBundle;

        public static readonly ItemType CloakMineralItemType = EnumUtils.Create<ItemType>("CloakMineral");
        public static readonly ItemType GhostbirdSkullItemType = EnumUtils.Create<ItemType>("GhostbirdSkull");
        public static readonly ItemType SealItemType = EnumUtils.Create<ItemType>("StrangerSeal");

        public void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        public interface IAchievements
        {
            void RegisterAchievement(string uniqueID, bool secret, ModBehaviour mod);
            void RegisterTranslation(string uniqueID, TextTranslation.Language language, string name, string description);
            void RegisterTranslationsFromFiles(ModBehaviour mod, string folderPath);
            void EarnAchievement(string uniqueID);
            bool HasAchievement(string uniqueID);
        }
        public static TheStrangerTheyAre Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<TheStrangerTheyAre>();
                return instance;
            }
        }

        private static TheStrangerTheyAre instance;

        public static void WriteLine(string text, MessageType messageType = MessageType.Message)
        {
            Instance.ModHelper.Console.WriteLine(text, messageType);
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(TheStrangerTheyAre)} is loaded!", MessageType.Success);
            var AchievementsAPI = ModHelper.Interaction.TryGetModApi<IAchievements>("xen.AchievementTracker");

            // Get the New Horizons API and load configs
            NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizonsAPI.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
            NewHorizonsAPI.LoadConfigs(this);

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene == OWScene.PostCreditsScene)
                {
                    if (endingBundle == null)
                    {
                        endingBundle = ModHelper.Assets.LoadBundle("assets/AssetBundle/postcredits");
                    }
                    if (endingBundle != null)
                    {
                        EndSceneAddition.LoadEndingAdditions(endingBundle);
                    }
                }
            };
        }

        public void OnStarSystemLoaded(string starSystem)
        {
            if (starSystem == "SolarSystem")
            {
                OnSolarSystemLoaded();
            }
            else if (starSystem == "AnonymousStrangerOW.StrangerSystem")
            {
                OnStrangerSystemLoaded();
            }
        }

        public void OnSolarSystemLoaded()
        {
            var preBramble = NewHorizonsAPI.GetPlanet("Pre Bramble");
            var preBrambleSector = preBramble.transform.Find("Sector");

            // Offset all children of the planet to match the ground model (includes GravityWell here)
            var offset = new Vector3(-10.1f, 246.8f, 99.9f);
            foreach (Transform child in preBramble.transform)
            {
                // Skip the sector because some of its children need to move and some don't
                if (child.name != "Sector")
                {
                    child.localPosition += offset;
                }
            }

            // Everything NH made under Sector (Fog, Air, AmbientLight) is centered so we offset them just like the ground model
            var childrenToOffset = new string[] { "AmbientLight", "Air", "FogSphere", "Atmosphere", "GroundSphere", "Water" };
            foreach (Transform child in preBrambleSector.transform)
            {
                if (childrenToOffset.Any(x => x == child.name))
                {
                    child.localPosition += offset;
                }
            }

            // Makes sure that artifacts get blown out when going under water
            Locator.GetPlayerBody().gameObject.AddComponent<HeldArtifactWaterHandler>();
        }

        public void OnStrangerSystemLoaded()
        {
            var ringedGiant = NewHorizonsAPI.GetPlanet("Ringed Giant");
            ringedGiant.transform.Find("Sector/Ring").GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 4001;

            var burningBombardier = NewHorizonsAPI.GetPlanet("Burning Bombardier");
            var detector = burningBombardier.GetComponentInChildren<ConstantForceDetector>();
            foreach (var meteorLauncher in burningBombardier.GetComponentsInChildren<MeteorLauncher>())
            {
                meteorLauncher.gameObject.SetActive(false);
                var veryActiveLauncher = meteorLauncher.gameObject.AddComponent<VeryActiveMeteorLauncher>();
                veryActiveLauncher._meteorPrefab = meteorLauncher._meteorPrefab;
                veryActiveLauncher._meteorPrefab.GetComponentInChildren<DynamicForceDetector>()._activeInheritedDetector = detector;
                veryActiveLauncher._dynamicMeteorPrefab = meteorLauncher._dynamicMeteorPrefab;
                veryActiveLauncher._dynamicProbability = meteorLauncher._dynamicProbability;
                veryActiveLauncher._audioSector = meteorLauncher._audioSector;
                veryActiveLauncher._minLaunchSpeed = meteorLauncher._minLaunchSpeed;
                veryActiveLauncher._maxLaunchSpeed = meteorLauncher._maxLaunchSpeed;
                veryActiveLauncher._minInterval = meteorLauncher._minInterval;
                veryActiveLauncher._maxInterval = meteorLauncher._maxInterval;
                veryActiveLauncher._launchParticles = meteorLauncher._launchParticles;
                veryActiveLauncher._launchSource = meteorLauncher._launchSource;
                veryActiveLauncher._launchDirection = meteorLauncher._launchDirection;
                GameObject.DestroyImmediate(meteorLauncher);
                veryActiveLauncher.gameObject.SetActive(true);
            }
        }

        public bool IsSeizureModeOn()
        {
            return ModHelper.Config.GetSettingsValue<bool>("Reduce Flashing Lights");
        }
    }
}
