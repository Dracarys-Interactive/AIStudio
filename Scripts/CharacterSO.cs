using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static Cinemachine.CinemachineBlendDefinition;

namespace DracarysInteractive.AIStudio
{
    [CreateAssetMenu]
    public class CharacterSO : ScriptableObject
    {
        public string character;
        public string voice;
        public string avatar;
        [TextArea(10, 100)]
        public string narrative;
        [TextArea(10, 100)]
        public string SSML;

        private void OnEnable()
        {
            // If voice is set but not SSML then generate SSML enriched with voice.
            if (string.IsNullOrEmpty(SSML) && !string.IsNullOrEmpty(voice))
            {
                SSML = @"
                    <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
                        <voice name='^voice^'>
                            ^text^
                        </voice>
                    </speak>
                ";
                SSML = SSML.Replace("^voice^", voice);
            }
        }
    }
}

