#if USE_MICROSOFT_COGNITIVESERVICES_SPEECH
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace DracarysInteractive.AIStudio
{
    public class MSSpeechServices : MonoBehaviour, ISpeechServices
    {
#if USE_MICROSOFT_COGNITIVESERVICES_SPEECH
        public string key;
        public string region;
        public float sampleRate = 24000;
        public float segmentationSilenceTimeout = 2;
        public bool stream = true;

        private Dictionary<string, SpeechSynthesizer> synthesizers = new Dictionary<string, SpeechSynthesizer>();
        private SpeechRecognizer recognizer;
        private List<VoiceInfo> voiceInfo;
        private bool _recognitionStarted;
        private string defaultVoice = "en-US-JennyNeural";
        private Action _onStartSpeechRecognition;
        private Action<string> _onSpeechRecognized;
        private Action _onSpeechNotRecognized;

#if PLATFORM_ANDROID
        // Required to manifest microphone permission, cf.
        // https://docs.unity3d.com/Manual/android-manifest.html
        private Microphone mic;
#endif

        public bool RecognitionStarted
        {
            get { return _recognitionStarted; }
        }

        private SpeechConfig getConfig()
        {
            if (key == null || key.Length == 0)
                key = Environment.GetEnvironmentVariable("COGNITIVE_SERVICE_KEY");

            if (region == null || region.Length == 0)
                region = Environment.GetEnvironmentVariable("COGNITIVE_SERVICE_REGION");

            return SpeechConfig.FromSubscription(key, region);
        }

        void Awake()
        {
            SpeechConfig config = getConfig();

            config.OutputFormat = OutputFormat.Simple;
            config.SetProperty(PropertyId.Speech_SegmentationSilenceTimeoutMs, (segmentationSilenceTimeout * 1000).ToString());

            var audioProcessingOptions = AudioProcessingOptions.Create(
                AudioProcessingConstants.AUDIO_INPUT_PROCESSING_ENABLE_DEFAULT,
                PresetMicrophoneArrayGeometry.Linear2,
                SpeakerReferenceChannel.LastChannel);
            var audioInput = AudioConfig.FromDefaultMicrophoneInput(audioProcessingOptions);

            // Azure speech services seem to have issue with various configurations of audio
            // processing.
            // recognizer = new SpeechRecognizer(config, audioInput);

            recognizer = new SpeechRecognizer(config);

#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif

            recognizer.Canceled += (s, e) =>
            {
                var cancellation = CancellationDetails.FromResult(e.Result);
                string message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
                SpeechServices.Instance.Log(message, SpeechServices.LogLevel.error);
            };

            recognizer.Recognizing += (s, e) =>
            {
                SpeechServices.Instance.Log($"RECOGNIZING: Text={e.Result.Text}");

                if (!_recognitionStarted)
                {
                    _recognitionStarted = true;
                    _onStartSpeechRecognition.Invoke();
                }
            };

            recognizer.Recognized += (s, e) =>
            {
                _recognitionStarted = false;

                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    SpeechServices.Instance.Log($"RECOGNIZED: Text={e.Result.Text}");
                    if (e.Result.Text.Length > 0)
                    {
                        _onSpeechRecognized(e.Result.Text);
                    }
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    SpeechServices.Instance.Log($"NOMATCH: Speech could not be recognized.");
                    _onSpeechNotRecognized();
                }
            };

            recognizer.SpeechEndDetected += (s, e) =>
            {
                _recognitionStarted = false;
                SpeechServices.Instance.Log($"Speech end detected.");
            };
        }

        public async Task<List<VoiceInfo>> RetrieveVoices()
        {
            SpeechSynthesizer synthesizer = GetSpeechSynthesizer(defaultVoice);
            List<VoiceInfo> voices = new List<VoiceInfo>();
            using var result = await synthesizer.GetVoicesAsync();
            if (result.Reason == ResultReason.VoicesListRetrieved)
            {
                foreach (var voice in result.Voices)
                {
                    voices.Add(voice);
                }
            }
            return voices;
        }

        public async void Speak(string text, string ssml, Action<float[]> onDataReceived, Action onSynthesisCompleted)
        {
            SpeechSynthesizer synthesizer = GetSpeechSynthesizer(ssml);

            ssml = ssml.Replace("^text^", text);
            SpeechServices.Instance.Log($"SSML: {ssml}");

            if (stream)
            {
                using (SpeechSynthesisResult result = await synthesizer.StartSpeakingSsmlAsync(ssml))
                {
                    using (var audioDataStream = AudioDataStream.FromResult(result))
                    {
                        byte[] buffer = new byte[(uint)sampleRate];
                        uint filledSize = 0;

                        while ((filledSize = audioDataStream.ReadData(buffer)) > 0)
                        {
                            onDataReceived(createAudioClipData(buffer, filledSize));
                        }
                    }
                }
            }
            else
            {
                Task<SpeechSynthesisResult> task = GetSpeechSynthesizer(ssml).SpeakSsmlAsync(ssml);
                task.Wait();
                onDataReceived(createAudioClipData(task.Result.AudioData, (uint)task.Result.AudioData.Length));
            }

            onSynthesisCompleted();
        }

        public async void Recognize(Action onStartSpeechRecognition, Action<string> onSpeechRecognized, Action onSpeechNotRecognized)
        {
            _onStartSpeechRecognition = onStartSpeechRecognition;
            _onSpeechRecognized = onSpeechRecognized;
            _onSpeechNotRecognized = onSpeechNotRecognized;

            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                SpeechServices.Instance.Log($"RECOGNIZED: Text={result.Text}");
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                SpeechServices.Instance.Log($"NOMATCH: Speech could not be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                SpeechServices.Instance.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    SpeechServices.Instance.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    SpeechServices.Instance.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    SpeechServices.Instance.Log($"CANCELED: Did you update the subscription info?");
                }
            }
        }

        public async void StartContinuousRecognizing(Action onStartSpeechRecognition, Action<string> onSpeechRecognized)
        {
            _onStartSpeechRecognition = onStartSpeechRecognition;
            _onSpeechRecognized = onSpeechRecognized;

            var stopRecognition = new TaskCompletionSource<int>();

            recognizer.Canceled += (s, e) =>
            {
                _recognitionStarted = false;

                SpeechServices.Instance.Log($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    SpeechServices.Instance.Log($"CANCELED: ErrorCode={e.ErrorCode}");
                    SpeechServices.Instance.Log($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    SpeechServices.Instance.Log($"CANCELED: Did you set the speech resource key and region values?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStopped += (s, e) =>
            {
                _recognitionStarted = false;
                SpeechServices.Instance.Log("\n    Session stopped event.");
                stopRecognition.TrySetResult(0);
            };

            await recognizer.StartContinuousRecognitionAsync();

            // Waits for completion. Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });
        }

        public async void StopContinuousRecognizing()
        {
            await recognizer.StopContinuousRecognitionAsync();
        }

        private SpeechSynthesizer GetSpeechSynthesizer(string ssml)
        {
            if (!synthesizers.ContainsKey(ssml))
            {
                SpeechConfig config = getConfig();

                // config.SpeechSynthesisVoiceName = voice;


                // The default format is RIFF, which has a riff header.
                // We are playing the audio in memory as audio clip, which doesn't require riff header.
                // So we need to set the format to raw (24KHz for better quality).
                config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

                // Creates a speech synthesizer.
                // Make sure to dispose the synthesizer after use!
                SpeechSynthesizer synthesizer = new SpeechSynthesizer(config, null);

                synthesizer.SynthesisCanceled += (s, e) =>
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
                    string message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
                    SpeechServices.Instance.Log(message, SpeechServices.LogLevel.error);
                };

                synthesizers[ssml] = synthesizer;
            }

            return synthesizers[ssml];
        }

        private static float[] createAudioClipData(byte[] audioChunkBytes, uint length)
        {
            uint size = length / 2;
            float[] audioChunk = new float[size];

            for (int i = 0; i < size; ++i)
            {
                if (i < size)
                {
                    audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                }
                else
                {
                    audioChunk[i] = 0.0f;
                }
            }

            return audioChunk;
        }

        void OnDestroy()
        {
            if (recognizer != null)
            {
                recognizer.Dispose();
            }

            foreach (var syn in synthesizers.Values)
            {
                syn.Dispose();
            }
        }

        public float SampleRate()
        {
            return sampleRate;
        }
#else
        public void Recognize(Action onStartSpeechRecognition, Action<string> onSpeechRecognized)
        {
            throw new NotImplementedException();
        }

        public float SampleRate()
        {
            throw new NotImplementedException();
        }

        public void Speak(string text, string voice, Action<float[]> onDataReceived, Action onSynthesisCompleted)
        {
            throw new NotImplementedException();
        }

        public void StartContinuousRecognizing(Action onStartSpeechRecognition, Action<string> onSpeechRecognized)
        {
            throw new NotImplementedException();
        }

        public void StopContinuousRecognizing()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
