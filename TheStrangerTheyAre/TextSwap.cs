using NewHorizons.Utility;
using UnityEngine;

namespace TheStrangerTheyAre
{
    public class TextSwap : MonoBehaviour
    {
        [SerializeField] public GameObject TranslatorText;
        [SerializeField] public GameObject Dialogue;

        private bool _isSwapped;
        private Collider _collider;

        public void Start()
        {
            _collider = GetComponent<Collider>();

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

            Apply(StrangerTextHandlerTSTA.KnowsLanguage());
            GlobalMessenger<string, bool>.AddListener("NHPersistentConditionChanged", OnPersistentConditionChanged);
        }

        public void OnDestroy()
        {
            GlobalMessenger<string, bool>.RemoveListener("NHPersistentConditionChanged", OnPersistentConditionChanged);
        }

        private void OnPersistentConditionChanged(string condition, bool value)
        {
            if (!_isSwapped && condition == StrangerTextHandlerTSTA.LANGUAGE_PC && value)
            {
                Apply(true);
            }
        }

        private void Apply(bool learnedLanguage)
        {
            TranslatorText.SetActive(false); // Ghost arcs are always disabled because a decal already exists
            Dialogue.SetActive(learnedLanguage);

            if (_collider != null)
            {
                _collider.enabled = !learnedLanguage;
            }

            _isSwapped = learnedLanguage;
        }
    }
}