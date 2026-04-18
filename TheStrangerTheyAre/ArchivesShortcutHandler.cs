using NewHorizons.Utility;
using System.Collections;
using UnityEngine;

namespace TheStrangerTheyAre
{
    public class ArchivesShortcutHandler : AbstractGearInterface
    {
        [SerializeField]
        public SpawnPoint brambleSpawn;

        private SpawnPoint _archivesSpawn;

        // spawn stuff
        private PlayerSpawner _spawner; // for spawning the player
        private bool _isTeleporting;

        public const float blinkTime = 0.5f; // constant for blink time
        public const float animTime = blinkTime / 2f; // constant for blink animation time

        public override void Start()
        {
            base.Start();
            _archivesSpawn = SearchUtilities.Find("PreBramble_Archives_Body/Sector/PreBrambleBase/Interactibles/BrambleArchivesSpawn").GetComponent<SpawnPoint>();
        }

        public override bool CanInteract => !_isTeleporting;

        public override void OnGearRotated()
        {
            if (_isTeleporting) return;
            StartCoroutine(UseShortcut());
        }

        private IEnumerator UseShortcut()
        {
            _isTeleporting = true;
            
            yield return new WaitForSeconds(0.5f); // let the animation play first
            OWInput.ChangeInputMode(InputMode.None); // stop player input for a while
            var cameraEffectController = FindObjectOfType<PlayerCameraEffectController>(); // gets camera controller

            // close eyes
            cameraEffectController.CloseEyes(animTime); // closes eyes
            yield return new WaitForSeconds(animTime);  // waits until animation stops to proceed to next line
            GlobalMessenger.FireEvent("PlayerBlink"); // fires an event for the player blinking

            // warp to archives
            _spawner = GameObject.FindGameObjectWithTag("Player").GetRequiredComponent<PlayerSpawner>(); // gets player spawner
            _spawner.DebugWarp(brambleSpawn); // warps you to bramble seed
            yield return new WaitForSeconds(2.5f); // waits for player to enter dimension
            _spawner.DebugWarp(_archivesSpawn); // warps you to archives

            // open eyes
            cameraEffectController.OpenEyes(animTime, false); // open eyes
            yield return new WaitForSeconds(animTime); //  waits until animation stops to proceed to next line
            OWInput.ChangeInputMode(InputMode.Character); // stop player input for a while
            Locator.GetShipLogManager().RevealFact("PREBRAMBLE_SHORTCUT_E"); // reveal shiplog

            _isTeleporting = false;
        }
    }
}
