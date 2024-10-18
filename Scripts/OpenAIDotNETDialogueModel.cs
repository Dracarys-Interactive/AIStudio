#if USE_COM_OPENAI_API
using OpenAI.Chat;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DracarysInteractive.AIStudio
{
    public class OpenAIDotNETDialogueModel : MonoBehaviour, IDialogueModel
    {
#if USE_COM_OPENAI_API
        public string model = "gpt-4o";
        public bool async = false;
        public string[] messages; // Inspector debugging...

        private ChatCompletionOptions _options = new ChatCompletionOptions();
        private ChatClient _client;
        private List<ChatMessage> _messages = new List<ChatMessage>();
        private List<string> _raw = new List<string>();

        void Awake()
        {
            DialogueModel.Instance.Log($"creating OpenAI .NET chat client, using model {model}");
            _client = new(model, Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        }

        private void Add(ChatMessage message)
        {
            _messages.Add(message);
            _raw.Add(message.Content[0].Text);
            messages = _raw.ToArray();
        }

        public void Clear()
        {
            DialogueModel.Instance.Log("enter Clear");
            _messages.Clear();
            messages = new string[0];
        }

        public async void Complete(Action<string> onResponse, Action<UnityWebRequest> onError)
        {
            if (async)
                await CompleteAsync(onResponse, onError);
            else
                CompleteSync(onResponse, onError);
        }

        private async Task CompleteAsync(Action<string> onResponse, Action<UnityWebRequest> onError)
        {
            DialogueModel.Instance.Log("enter CompleteAsync");

            ChatCompletion completion = await _client.CompleteChatAsync(_messages, _options);
            string text = completion.Content[0].Text;

            Add(text);
            onResponse.Invoke(text);
        }

        private void CompleteSync(Action<string> onResponse, Action<UnityWebRequest> onError)
        {
            DialogueModel.Instance.Log("enter CompleteSync");

            ChatCompletion completion = _client.CompleteChat(_messages, _options);
            string text = completion.Content[0].Text;

            Add(text);
            onResponse.Invoke(text);
        }


        public void Prompt(string prompt)
        {
            DialogueModel.Instance.Log("enter Prompt");
            Add(ChatMessage.CreateSystemMessage(prompt));
        }

        public void Speak(string text)
        {
            DialogueModel.Instance.Log("enter Speak");
            Add(ChatMessage.CreateUserMessage(text));
        }

        public void Respond(string response)
        {
            DialogueModel.Instance.Log("enter Respond");
            Add(ChatMessage.CreateAssistantMessage(response));
        }

        /*
        private void add(string content, string role)
        {
            DialogueModel.Instance.Log("enter add");
            MessageV1 message = new MessageV1();
            message.content = content;
            message.role = role;
            OpenAiChatCompleterV1.Instance.dialogue.Add(message);
        }
        */
#else
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Complete(Action<string> onResponse, Action<UnityWebRequest> onError)
        {
            throw new NotImplementedException();
        }

        public void Prompt(string prompt)
        {
            throw new NotImplementedException();
        }

        public void Respond(string text)
        {
            throw new NotImplementedException();
        }

        public void Speak(string text)
        {
            throw new NotImplementedException();
        }
#endif
    }
}