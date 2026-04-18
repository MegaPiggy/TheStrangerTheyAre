using NewHorizons.Utility;
using UnityEngine;

namespace TheStrangerTheyAre
{
    public class StrangerTextHandlerTSTA : MonoBehaviour
    {
        private GameObject[] _strangerDialogue = new GameObject[11]; // create new array of gameobjects to store all custom sim reels

        public void Start()
        {
            for (int i = 0; i < 11; i++)
            {
                _strangerDialogue[i] = SearchUtilities.Find("TSTA_StrangerDialogue_" + (i + 1)); // gets all custom stranger dialogue in the sim, stores in array
            }
        }

        public static bool KnowsLanguage()
        {
            return Locator.GetShipLogManager().IsFactRevealed("ANGLERS_EYE_ALIENTEXT_E2");
        }

        public void Update()
        {
            foreach (GameObject dialogue in _strangerDialogue)
            {
                dialogue.SetActive(KnowsLanguage());
            }
        }
    }
}
