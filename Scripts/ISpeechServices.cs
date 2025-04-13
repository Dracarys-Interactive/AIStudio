using System;
using System.Threading.Tasks;

namespace DracarysInteractive.AIStudio
{
    public interface ISpeechServices
    {
        void StartContinuousRecognizing(Action onStartSpeechRecognition, Action<string> onSpeechRecognized);
        void StopContinuousRecognizing();
        void Recognize(Action onStartSpeechRecognition, Action<string> onSpeechRecognized, Action onSpeechNotRecognized);
        void Speak(string text, string ssml, Action<float[]> onDataReceived, Action onSynthesisCompleted);
        float SampleRate();
    }
}
