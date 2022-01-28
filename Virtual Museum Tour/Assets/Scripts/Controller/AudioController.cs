using JetBrains.Annotations;
using UnityEngine.Audio;
using System.Collections;
using System.Linq;
using Controller.Audio;
using UnityEngine;

namespace Controller
{
    /// <summary>
    /// An controller that handles all the audio clips which should be played.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        // Constants
        private const string EnvironmentAudioPath = "AudioClips/Environment/";
        private const string MusicPath = "AudioClips/Music/";
        private const string EffectsAudioPath = "AudioClips/Effects/";
        private const string TestAudioPath = "AudioClips/Test/";

        // Serialize fields
        [SerializeField] private AudioMixerGroup storytellingMixerGroup;
        [SerializeField] private AudioMixerGroup environmentMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        // Static members
        public static AudioController Instance { get; private set; }

        // Members
        private AudioSource _storytellingAudioSource;
        
        private AudioQueue _environmentAudioQueue;
        private AudioQueue _musicAudioQueue;
        
        private Coroutine _environmentCoroutine;
        private Coroutine _musicCoroutine;

        #region Unity Functions

        private void Awake()
        {
            // Set instance
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            // Setup background audio
            // Load audio clips from directory
            AudioClip[] environmentClipsFromDirectory = LoadAudioClipsFromDirectory(EnvironmentAudioPath);
            _environmentAudioQueue = new AudioQueue(environmentClipsFromDirectory, AudioQueueType.Environment);
            // Start background audio loop
            _environmentCoroutine = StartCoroutine(AudioQueueCoroutine(_environmentAudioQueue));

            // setup music audio
            // load audio clips from directory
            AudioClip[] musicClipsFromDirectory = LoadAudioClipsFromDirectory(MusicPath);
            _musicAudioQueue = new AudioQueue(musicClipsFromDirectory, AudioQueueType.Music);
            // Start music audio loop
            _musicCoroutine = StartCoroutine(AudioQueueCoroutine(_musicAudioQueue));
            
            // Setup storytelling audio
            _storytellingAudioSource = gameObject.AddComponent<AudioSource>(); // create new audio source
            _storytellingAudioSource.playOnAwake = false;
            _storytellingAudioSource.outputAudioMixerGroup = storytellingMixerGroup; // set storytelling mixer group
        }

        /*private void Update()
        {
            if (_audioSource.isPlaying || _audioClipQueue.Count <= 0 || _audioPlayRoutine != null) return;
        
            var clip = GetNextAudioClipFromQueue();
            _audioPlayRoutine = StartCoroutine(PlayAudioRoutine(clip));
        }*/

        private void OnDestroy()
        {
            if (Instance != null && Instance == this) Instance = null;
        }

        #endregion

        #region AudioController specific methods

        public void AddAudioToQueue([NotNull] AudioClip clip, AudioQueueType type)
        {
            switch (type)
            {
                case AudioQueueType.Environment:
                    _environmentAudioQueue.Add(clip);
                    break;
                case AudioQueueType.Music:
                    _musicAudioQueue.Add(clip);
                    break;
                default:
                    Debug.LogError($"[{nameof(AddAudioToQueue)}] Error while trying to add audio to queue.");
                    break;
            }
        }

        /// <summary>
        /// Audio loop.
        /// </summary>
        /// <param name="queue">Queue for referencing</param>
        private IEnumerator AudioQueueCoroutine([NotNull] AudioQueue queue)
        {
            if (queue.Length <= 0) yield break; // if queue is empty, break out of coroutine

            AudioSource audioSource = gameObject.AddComponent<AudioSource>(); // add new audio source component
            audioSource.playOnAwake = false; // disable play on start
            
            // set correct audio mixer group
            switch (queue.QueueType)
            {
                case AudioQueueType.Environment:
                    audioSource.outputAudioMixerGroup = environmentMixerGroup; // Environment mixer
                    break;
                case AudioQueueType.Music:
                    audioSource.outputAudioMixerGroup = musicMixerGroup; // Music mixer
                    break;
                default:
                    Debug.LogError($"[{nameof(AddAudioToQueue)}] Error while trying to determine mixer group.");
                    break;
            }
            
            while (true)
            {
                Debug.Log($"[{nameof(AudioQueueCoroutine)}]: Run coroutine with queue length of {queue.Length}");
                
                AudioClip audioClip = queue.GetNextAudioClipFromQueue(); // load next clip from queue

                Debug.Log($"[{nameof(AudioQueueCoroutine)}]: AudioClip: <color=#FF0000>{audioClip.name}</color> will be played next.");
                
                audioSource.clip = audioClip; // set loaded audio clip to audio source

                audioSource.Play(); // finally, play audio

                yield return new WaitUntil(() => audioSource.isPlaying == false); // wait until current audio clip finished playing
            }
        }

        public void PlayStorytellingAudio([NotNull] AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("AudioClip can't be null.");
                return;
            }

            if (_storytellingAudioSource.isPlaying) // if there is currently audio playing
            {
                _storytellingAudioSource.Stop();
            }

            #if UNITY_EDITOR
                _storytellingAudioSource.clip = Resources.LoadAll<AudioClip>(TestAudioPath)[0];
            #else
                _storytellingAudioSource.clip = clip;
            #endif
            
            _storytellingAudioSource.Play();
        }

        #endregion

        #region Resource Management

        private static AudioClip[] LoadAudioClipsFromDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            var clips = Resources.LoadAll(path, typeof(AudioClip));

            return clips
                .Cast<AudioClip>()
                .Where(clip => clip != null)
                .ToArray();
        }

        #endregion
    }
}