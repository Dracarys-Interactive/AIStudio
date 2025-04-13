using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public abstract class DialogueTrigger : MonoBehaviour
    {
        public DialogueSO dialogue;
        public bool _triggered = false;

        protected void OnTrigger()
        {
            if (!Triggered)
            {
                Triggered = true;
                DialogueManager.Instance.StartDialogue(dialogue);
            }
        }

        public bool Triggered
        {
            get { return _triggered; }
            set { _triggered = value; }
        }
    }
}