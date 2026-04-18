using NewHorizons.Utility;
using UnityEngine;

namespace TheStrangerTheyAre
{
    public class TextSwap : MonoBehaviour
    {
        [SerializeField] public GameObject TranslatorText;
        [SerializeField] public GameObject Dialogue;

        private bool _isSwapped;

        public void Start()
        {
            if (Dialogue == null)
            {
                TheStrangerTheyAre.WriteLine($"TextSwap at {transform.GetPath()} is missing a reference to the dialogue.", OWML.Common.MessageType.Error);
                Dialogue = gameObject.FindChild("Dialogue");
            }

            if (TranslatorText == null)
            {
                TheStrangerTheyAre.WriteLine($"TextSwap at {transform.GetPath()} is missing a reference to the translator text.", OWML.Common.MessageType.Error);
                TranslatorText = gameObject.FindChild("Arc 1");
            }

            Apply(Check());
        }

        public void Update()
        {
            if (!_isSwapped && Check())
            {
                Apply(true);
            }
        }

        private void Apply(bool learnedLanguage)
        {
            // Always disable translator text in DreamWorld
            TranslatorText.SetActive(!IsInDreamWorld() && !learnedLanguage);
            Dialogue.SetActive(learnedLanguage);
            _isSwapped = learnedLanguage;
        }

        private bool Check()
        {
            return Locator.GetShipLogManager().IsFactRevealed("ANGLERS_EYE_ALIENTEXT_E2");
        }

        private bool IsInDreamWorld()
        {
            return this.GetAttachedOWRigidbody().gameObject.name.StartsWith("DreamWorld");
        }
    }
}