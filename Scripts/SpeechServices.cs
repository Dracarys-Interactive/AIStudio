using Microsoft.CognitiveServices.Speech;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace DracarysInteractive.AIStudio
{
    [RequireComponent(typeof(ISpeechServices))]
    public class SpeechServices : Pluggable<SpeechServices, ISpeechServices>
    {
        public bool recognizeContinuously = false;
        public bool recognizeAfterEndSpeaking = false;
        public GameObject warningText;
        public bool recognize = false;

        protected override void Awake()
        {
            // Force instantiation of plug-in otherwise we
            // might call GetComponent in a subthread.
            var bootImplementation = Implementation;

            if (recognizeAfterEndSpeaking)
                DialogueCharacter.onEndSpeaking.AddListener(OnEndSpeaking);
        }

        private void OnEndSpeaking(DialogueCharacter arg0)
        {
            recognize = true;
        }

        public void StartContinuousRecognizing(Action onStartSpeechRecognition, Action<string> onSpeechRecognized)
        {
            Implementation.StartContinuousRecognizing(onStartSpeechRecognition, onSpeechRecognized);
        }

        public void StopContinuousRecognizing()
        {
            Implementation.StopContinuousRecognizing();
        }

        public void Recognize(Action onStartSpeechRecognition, Action<string> onSpeechRecognized)
        {
            Implementation.Recognize(onStartSpeechRecognition, onSpeechRecognized, onSpeechNotRecognized);
        }

        private void onSpeechNotRecognized()
        {
            recognize = true;
        }

        public void Speak(string text, string voice, Action<float[]> onDataReceived, Action onSynthesisCompleted)
        {
            Log($"Speak: {text}");
            Implementation.Speak(text, voice, onDataReceived, onSynthesisCompleted);
        }
        public float SampleRate()
        {
            return Implementation.SampleRate();
        }

        void Update()
        {
            if (warningText)
            {
                warningText.SetActive(!recognizeContinuously);
            }

            if (!recognizeContinuously && recognize)
            {
                recognize = false;
                Implementation.Recognize(() =>
                {
                    DialogueActionManager.Instance.EnqueueAction(new StartSpeechRecognition(DialogueManager.Instance.GetPlayer()));
                },
                (string text) =>
                {
                    Log($"recognized speech: {text}");
                    DialogueActionManager.Instance.EnqueueAction(new SpeechRecognized(DialogueManager.Instance.GetPlayer(), text));
                },
                () =>
                {
                    Log("speech NOT recognized, retry...");
                    recognize = true;
                });
            }
        }
      
        public void OnSpeak(CallbackContext context)
        {
            Log("enter OnSpeak", LogLevel.debug);

            if (context.performed)
                recognize = true;
        }

    }
}
