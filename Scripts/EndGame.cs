using UnityEditor;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public class EndGame : MonoBehaviour
    {
        public AudioSource sfx;
        public float sfxDelay = 2.5f;
        public float endDelay = 5f;

        void Start()
        {
            if (sfx)
                sfx.PlayDelayed(sfxDelay);

            Invoke("endGame", endDelay);
        }

        private void endGame()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
            else
#endif
                Application.Quit(0);
        }
    }
}