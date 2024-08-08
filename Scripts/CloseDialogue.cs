using System;
using UnityEngine.SceneManagement;

namespace DracarysInteractive.AIStudio
{
    public class CloseDialogue : DialogueAction<(string nextScene, int delay)>
    {
        public CloseDialogue(string nextScene = null, int delay = 0, Action onCompletion = null) : base(onCompletion)
        {
            data = (nextScene, delay);
        }

        public override void Invoke()
        {
            DialogueManager.Instance.dialogueClosed = true;

            if (onCompletion != null)
                onCompletion.Invoke();

            if (DialogueActionManager.Instance.ActionsPending)
            {
                DialogueActionManager.Instance.EnqueueAction(this, true);
            }
            else if (data.nextScene != null && data.nextScene.Trim().Length > 0)
            {
                SceneManager.LoadSceneAsync(data.nextScene);
            }
        }
    }
}