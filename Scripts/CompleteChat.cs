using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace DracarysInteractive.AIStudio
{
    public class CompleteChat : DialogueAction<(string text, bool prompt, Action<string> onResponse)>
    {
        private const int MAX_RETRIES = 2;
        private int retries = 0;

        public CompleteChat()
        {
        }

        public CompleteChat(string text, bool prompt = false, Action onCompletion = null, Action<string> onResponse = null) : base(onCompletion)
        {
            data = (text, prompt, onResponse);
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
            if (data.onResponse != null)
            {
                data.onResponse.Invoke(completion);
            }

            retries = 0;

            Log($"OnChatCompletion: completion={completion}");

            /*
            foreach (string tag in DialogueManager.Instance.tags)
            {
                string[] taggedStrings = StringHelper.ExtractTaggedStrings(completion, tag);
                if (taggedStrings.Length > 0)
                {
                    DialogueActionManager.Instance.EnqueueAction(new ProcessTags(null, tag, taggedStrings));
                    completion = StringHelper.RemoveTaggedStrings(completion, tag);
                    Log($"OnChatCompletion: completion after tag {tag} processing={completion}");
                }
            }
            */

            string[] subcompletions = StringHelper.SplitCompletion(completion, DialogueManager.Instance.nameDelimiter);
            string name = null;

            for (int j = 0; j < subcompletions.Length; j++)
            {
                string subcompletion = subcompletions[j];

                if (subcompletion.Trim().Length == 0)
                    continue;

                Log($"CompleteChat.OnChatCompletion subcompletion=\"{subcompletion}\"");

                int i = subcompletion.IndexOf(DialogueManager.Instance.nameDelimiter);

                if (i != -1)
                {
                    name = subcompletion.Substring(0, i);
                }

                DialogueCharacter npc = DialogueManager.Instance.GetNPC(name);

                if (npc == null)
                {
                    Log($"CompleteChat.OnChatCompletion model responded as player {name}", Singleton<DialogueActionManager>.LogLevel.warning);
                    continue;
                }

                foreach (string tag in DialogueManager.Instance.tags)
                {
                    string[] taggedStrings = StringHelper.ExtractTaggedStrings(subcompletion, tag);

                    if (taggedStrings.Length > 0)
                    {
                        DialogueActionManager.Instance.EnqueueAction(new ProcessTags(npc, tag, taggedStrings));
                        subcompletion = StringHelper.RemoveTaggedStrings(subcompletion, tag);
                        Log($"OnChatCompletion: subcompletion after tag {tag} processing={subcompletion}");
                    }
                }

                Log($"OnChatCompletion: subcompletion after tag processing: {subcompletion}");

                subcompletion = StringHelper.filterSubcompletion(subcompletion);

                Log($"OnChatCompletion: subcompletion after filtering: {subcompletion}");

                string[] actions = StringHelper.ExtractStringsInParentheses(subcompletion);

                Log($"OnChatCompletion: Speak: {subcompletion}");

                DialogueActionManager.Instance.EnqueueAction(new Speak(npc, subcompletion, actions));
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