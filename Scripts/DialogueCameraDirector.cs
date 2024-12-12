using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    [RequireComponent(typeof(IDialogueCameraDirector))]
    public class DialogueCameraDirector : Pluggable<DialogueCameraDirector, IDialogueCameraDirector>
    {
        protected override void Awake()
        {
            base.Awake();

            DialogueCharacter.onStartSpeechRecognition.AddListener(OnStartSpeechRecognition);
            DialogueCharacter.onSpeechRecognized.AddListener(OnSpeechRecognized);
            DialogueCharacter.onStartSpeaking.AddListener(OnStartSpeaking);
            DialogueCharacter.onEndSpeaking.AddListener(OnEndSpeaking);
        }

        public void OnStartSpeechRecognition(DialogueCharacter character)
        {
            Implementation.OnStartSpeechRecognition(character);
        }
        void OnSpeechRecognized(DialogueCharacter character)
        {
            Implementation.OnStartSpeechRecognition(character);
        }
        void OnStartSpeaking(DialogueCharacter character, string text)
        {
            Implementation.OnStartSpeechRecognition(character);
        }
        void OnEndSpeaking(DialogueCharacter character)
        {
            Implementation.OnStartSpeechRecognition(character);
        }
    }
}
