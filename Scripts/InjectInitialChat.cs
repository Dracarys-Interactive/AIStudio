using System;

namespace DracarysInteractive.AIStudio
{
    public class InjectInitialChat : CompleteChat
    {
        public InjectInitialChat(string text, Action onCompletion = null) : base(text, true, onCompletion)
        {
        }
        public override void Invoke()
        {
            DialogueModel.Instance.Respond(data.text);
            OnChatCompletion(data.text);
        }
    }
}