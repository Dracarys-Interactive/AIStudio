using System;
using UnityEngine.Events;

namespace DracarysInteractive.AIStudio
{
    [Serializable]
    public class ProcessTags : DialogueAction<(DialogueCharacter character, string tag, string[] strings)>
    {
        public static UnityEvent<DialogueCharacter, string, string[]> OnProcessTags = new UnityEvent<DialogueCharacter, string, string[]>();

        public ProcessTags(DialogueCharacter character, string tag, string[] strings = null, Action onCompletion = null) : base(onCompletion)
        {
            data = (character, tag, strings);
        }

        public override void Invoke()
        {
            OnProcessTags.Invoke(data.character, data.tag, data.strings);

            if (onCompletion != null)
                onCompletion.Invoke();
        }

    }
}