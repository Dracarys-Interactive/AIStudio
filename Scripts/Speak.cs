using System;

namespace DracarysInteractive.AIStudio
{
    [Serializable]
    public class Speak : DialogueAction<(DialogueCharacter character, string text, string[] actions)>
    {
        public Speak(DialogueCharacter character, string text, string[] actions = null, Action onCompletion = null) : base(onCompletion)
        {
            data = (character, text, actions);
        }

        public override void Invoke()
        {
            DialogueActionManager.Instance.EnqueueAction(new StartSpeechSynthesis(data));
            string text = StringHelper.Remove(StringHelper.RemoveStringsInParentheses(data.text), "^.*:");
            foreach (string tag in DialogueManager.Instance.tags)
            {
                text = StringHelper.RemoveTaggedStrings(text, tag);
            }
            SpeechServices.Instance.Speak(text, data.character.character.voice, playAudio, onSynthesisCompleted);
        }

        private void playAudio(float[] result)
        {
            DialogueActionManager.Instance.EnqueueAction(new PlayAudio(data.character, result));
        }

        private void onSynthesisCompleted()
        {
            DialogueActionManager.Instance.EnqueueAction(new SpeechSynthesized(data.character));

            if (onCompletion != null)
                onCompletion.Invoke();
        }
    }
}