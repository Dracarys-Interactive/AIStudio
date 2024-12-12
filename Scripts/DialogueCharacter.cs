using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DracarysInteractive.AIStudio
{
    public class DialogueCharacter : MonoBehaviour
    {
        public static UnityEvent<DialogueCharacter, string> onStartSpeaking = new UnityEvent<DialogueCharacter, string>();
        public static UnityEvent<DialogueCharacter> onEndSpeaking = new UnityEvent<DialogueCharacter>();
        public static UnityEvent<DialogueCharacter> onStartSpeechRecognition = new UnityEvent<DialogueCharacter>();
        public static UnityEvent<DialogueCharacter> onSpeechRecognized = new UnityEvent<DialogueCharacter>();

        public CharacterSO character;
        public Text transcript;
        public Transform lookAtTarget;
        public bool usePlayerVoice = false;
        public bool removeActionsFromTranscript = true;

        private IDialogueCharacterAnimator[] _animators;

        void Awake()
        {
            _animators = GetComponents<IDialogueCharacterAnimator>();
            onStartSpeaking.AddListener(OnStartSpeaking);
            onEndSpeaking.AddListener(OnEndSpeaking);
            onStartSpeechRecognition.AddListener(OnStartSpeechRecognition);
            onSpeechRecognized.AddListener(OnSpeechRecognized);
        }

        private void OnSpeechRecognized(DialogueCharacter player)
        {
            foreach (IDialogueCharacterAnimator animator in _animators)
                animator.OnSpeechRecognized(this, player);
        }

        private void OnStartSpeechRecognition(DialogueCharacter player)
        {
            foreach (IDialogueCharacterAnimator animator in _animators)
                animator.OnStartSpeechRecognition(this, player);
        }

        private void OnEndSpeaking(DialogueCharacter speaker)
        {
            foreach (IDialogueCharacterAnimator animator in _animators)
                animator.OnEndSpeaking(this, speaker);
        }

        private void OnStartSpeaking(DialogueCharacter speaker, string text)
        {
            foreach (IDialogueCharacterAnimator animator in _animators)
                animator.OnStartSpeaking(this, speaker);
        }

        public void OnStartSpeechRecognition()
        {
            onStartSpeechRecognition.Invoke(this);
        }

        public void OnSpeechRecognized(string text)
        {
            onSpeechRecognized.Invoke(this);

            text = character.character + DialogueManager.Instance.nameDelimiter + text;

            if (transcript)
                transcript.text = text;

            if (usePlayerVoice)
                DialogueActionManager.Instance.EnqueueAction(new Speak(this, text));

            DialogueActionManager.Instance.EnqueueAction(new CompleteChat(text));
        }

        public void OnStartSpeaking(string text, string[] actions)
        {
            onStartSpeaking.Invoke(this, text);

            foreach (IDialogueCharacterAnimator animator in _animators)
                animator.AnimateActions(this, actions);

            if (transcript)
            {
                transcript.text = removeActionsFromTranscript ? StringHelper.RemoveStringsInParentheses(text) : text;
            }
        }

        public void OnEndSpeaking()
        {
            onEndSpeaking.Invoke(this);
        }

    }
}