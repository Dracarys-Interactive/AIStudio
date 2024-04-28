using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DracarysInteractive.AIStudio
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public DialogueSO activeDialogue;
        public UnityEvent ClosingPromptInjected;
        public bool dialogueClosed = false;
        public string[] tags;

        private DialogueCharacter _player;
        private Dictionary<string, DialogueCharacter> _NPCs = new Dictionary<string, DialogueCharacter>();
        private bool _isRecognizing = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += onSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= onSceneLoaded;
        }

        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            dialogueClosed = false;

            if (activeDialogue)
            {
                DialogueModel.Instance.Clear(); // flag in DialogeSO?
                DialogueActionManager.Instance.Clear();
                StartDialogue(activeDialogue);
            }
        }

        private void OnClosingPromptInjected()
        {
            DialogueActionManager.Instance.EnqueueAction(new CloseDialogue(activeDialogue.nextScene, 0, OnClosingPrompt));
        }

        private void OnClosingPrompt()
        {
            dialogueClosed = true;
            ClosingPromptInjected.Invoke();
        }

        public void StartDialogue(Dropdown dropdown)
        {
            string dialogueSOName = dropdown.options[dropdown.value].text;
            DialogueSO dialog = Resources.Load("Dialogues/" + dialogueSOName) as DialogueSO;
            StartDialogue(dialog);
        }

        public void StartDialogue(DialogueSO dialogue)
        {
            DialogueActionManager.Instance.Clear();

            activeDialogue = dialogue;

            _player = null;
            _NPCs.Clear();

            foreach (CharacterSO npcso in dialogue.nonPlayerCharacters)
            {
                _NPCs.Add(npcso.character, null);
            }

            DialogueCharacter[] characters = FindObjectsOfType<DialogueCharacter>();

            foreach (DialogueCharacter character in characters)
            {
                if (dialogue.player && dialogue.player.character == character.character.character)
                {
                    _player = character;
                }
                else if (_NPCs.ContainsKey(character.character.character))
                {
                    _NPCs[character.character.character] = character;
                }
            }

            if (dialogue.player)
            {
                if (!_isRecognizing && SpeechServices.Instance.recognizeContinuously)
                {
                    Task.Run(() => SpeechServices.Instance.StartContinuousRecognizing(onStartSpeechRecognition, onSpeechRecognized));
                    _isRecognizing = true;
                }

                DialogueModel.Instance.Prompt(dialogue.player.narrative);
            }
            else
            {
                if (_isRecognizing)
                {
                    Task.Run(SpeechServices.Instance.StopContinuousRecognizing);
                    _isRecognizing = false;
                }
            }

            Debug.Assert(_NPCs.Count == dialogue.nonPlayerCharacters.Count);

            foreach (CharacterSO npcso in dialogue.nonPlayerCharacters)
            {
                //Debug.Assert(_NPCs.ContainsKey(npcso.character));
                if (_NPCs.ContainsKey(npcso.character))
                {
                    DialogueCharacter npc = _NPCs[npcso.character];
                    if (npc)
                    {
                        npc.character = npcso;
                        DialogueModel.Instance.Prompt(npcso.narrative);
                    }
                }
                else
                {
                    Debug.LogWarning($"Dialogue.StartDialogue: no character {npcso.character}");
                }
            }

            DialogueModel.Instance.Prompt(dialogue.dialogueContext);

            if (dialogue.initialPrompt != null && dialogue.initialPrompt.Trim().Length > 0)
            {
                DialogueActionManager.Instance.EnqueueAction(new InjectChat(dialogue.initialPrompt, true));
            }

            if (dialogue.closingPrompt != null && dialogue.closingPrompt.Trim().Length > 0)
            {
                DialogueActionManager.Instance.EnqueueAction(new FutureAction(
                   new InjectChat(activeDialogue.closingPrompt, false, OnClosingPromptInjected), dialogue.closingPromptDelay));
            }
            else if (dialogue.closingPromptDelay > 0)
            {
                DialogueActionManager.Instance.EnqueueAction(new FutureAction(
                    new CloseDialogue(activeDialogue.nextScene, 0, OnClosingPrompt), dialogue.closingPromptDelay));
            }
        }
        private void onStartSpeechRecognition()
        {
            DialogueActionManager.Instance.EnqueueAction(new StartSpeechRecognition(Instance.GetPlayer()));
        }

        private void onSpeechRecognized(string text)
        {
            DialogueActionManager.Instance.EnqueueAction(new SpeechRecognized(Instance.GetPlayer(), text));
        }

        public DialogueCharacter GetNPC(string name)
        {
            if (name != null && _NPCs.ContainsKey(name))
            {
                return _NPCs[name];
            }
            return null;
        }

        public DialogueCharacter GetPlayer()
        {
            return _player;
        }

        public bool HasPlayer
        {
            get { return _player != null; }
        }

        public bool HasMultipleNPCs
        {
            get { return _NPCs.Count > 1; }
        }
    }
}
