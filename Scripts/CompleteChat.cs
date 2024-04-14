using System;
using UnityEngine;
using UnityEngine.Networking;

namespace DracarysInteractive.AIStudio
{
    public class CompleteChat : DialogueAction<(string text, bool prompt)>
    {
        private const int MAX_RETRIES = 2;
        private int retries = 0;

        public CompleteChat()
        {
        }

        public CompleteChat(string text, bool prompt = false, Action onCompletion = null) : base(onCompletion)
        {
            data = (text, prompt);
        }

        public override void Invoke()
        {
            if (data.text != null)
            {
                if (data.prompt)
                {
                    DialogueModel.Instance.Prompt(data.text);
                }
                else
                {
                    DialogueModel.Instance.Speak(data.text);
                    DialogueModel.Instance.Complete(OnChatCompletion, OnChatCompletionError);
                }
            }
            else
            {
                DialogueModel.Instance.Complete(OnChatCompletion, OnChatCompletionError);
            }
        }

        private void OnChatCompletionError(UnityWebRequest obj)
        {
            Log("OnChatCompletionError called!", DialogueActionManager.LogLevel.warning);

            if (retries++ < MAX_RETRIES)
            {
                Debug.Log($"retry # {retries}...");
                Invoke();
            }
            else
                Log("OnChatCompletionError called, retries exhausted!", DialogueActionManager.LogLevel.error);
        }

        public void OnChatCompletion(string completion)
        {
            retries = 0;

            Log($"OnChatCompletion: completion={completion}");

            string[] subcompletions = StringHelper.SplitCompletion(completion);

            for (int j = 0; j < subcompletions.Length; j++)
            {
                string subcompletion = subcompletions[j];

                if (subcompletion.Trim().Length == 0)
                    continue;

                Log($"CompleteChat.OnChatCompletion subcompletion=\"{subcompletion}\"");

                string name = null;
                int i = subcompletion.IndexOf(':');

                if (i != -1)
                {
                    name = subcompletion.Substring(0, i);
                }

                DialogueCharacter npc = DialogueManager.Instance.GetNPC(name);

                if (npc == null)
                {
                    Log($"CompleteChat.OnChatCompletion model responded as player {name}");
                    continue;
                }

                string[] actions = StringHelper.ExtractStringsInParentheses(subcompletion);

                DialogueActionManager.Instance.EnqueueAction(new Speak(npc, subcompletion, actions));

                foreach (string tag in DialogueManager.Instance.tags)
                {
                    string[] taggedStrings = StringHelper.ExtractTaggedStrings(subcompletion, tag);
                    DialogueActionManager.Instance.EnqueueAction(new ProcessTags(npc, tag, taggedStrings));
                }
            }

            if (!DialogueManager.Instance.HasPlayer)
            {
                DialogueActionManager.Instance.EnqueueAction(new CompleteChat());
            }

            if (onCompletion != null)
                onCompletion.Invoke();
        }
    }
}