using System;
using System.Collections;
using UnityEngine;

namespace DracarysInteractive.AIStudio
{
    public class PlayAudio : DialogueAction<(DialogueCharacter character, float[] result)>
    {
        public PlayAudio(DialogueCharacter character, float[] result, Action onCompleted = null) : base(onCompleted)
        {
            data = (character, result);
        }

        public override void Invoke()
        {
            if (data.result.Length == 0)
            {
                if (onCompletion != null)
                    onCompletion.Invoke();
            }
            else
            {
                AudioClip audioClip = AudioClip.Create(
                      data.character.character + " Speech",
                      data.result.Length,
                      1,
                      (int)SpeechServices.Instance.SampleRate(),
                      false);
                audioClip.SetData(data.result, 0);

                AudioSource audioSource = data.character.GetComponent<AudioSource>();

                audioSource.clip = audioClip;
                audioSource.Play();

                data.character.StartCoroutine(WaitForAudio(data.character, audioSource));
            }
        }

        IEnumerator WaitForAudio(DialogueCharacter character, AudioSource audioSource)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);

            if (onCompletion != null)
                onCompletion.Invoke();
        }
    }
}