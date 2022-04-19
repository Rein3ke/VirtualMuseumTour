using System;
using System.Collections;
using System.Linq;
using Controller;
using Events;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using EventType = Events.EventType;

namespace Audio
{
    /// <summary>
    /// An controller that handles all the audio clips which should be played.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        #region Constants

        private const string EnvironmentAudioPath = "AudioClips/Environment/";
        private const string MusicPath = "AudioClips/Music/";
        private const string EffectsAudioPath = "AudioClips/Effects/";
        private const string TestAudioPath = "AudioClips/Test/";
        private const float MasterVolumeHorizontalSliderWidth = 96f;
        
        #endregion

        #region SerializeFields

        [SerializeField] private AudioMixerGroup masterMixerGroup;
        [SerializeField] private AudioMixerGroup storytellingMixerGroup;
        [SerializeField] private AudioMixerGroup environmentMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        #endregion

        #region Members

        private AudioSource _storytellingAudioSource;
        
        private AudioQueue _environmentAudioQueue;
        private AudioQueue _musicAudioQueue;
        
        private Coroutine _environmentCoroutine;
        private Coroutine _musicCoroutine;
        private Coroutine _setMasterVolumeCoroutine;
        
        private float masterVolumeSliderValue;
        private bool _displayOnGUI;

        private float MasterVolumeSliderValue
        {
            get => masterVolumeSliderValue;
            set
            {
                masterVolumeSliderValue = value;
                AudioListener.volume = value;
                PlayerPrefs.SetFloat("MasterVolume", value);
            }
        }

        #endregion

        #region Unity Functions

        private void Awake()
        {
            // Setup background audio
            AudioClip[] environmentClipsFromDirectory = LoadAudioClipsFromDirectory(EnvironmentAudioPath); // Load audio clips from directory
            _environmentAudioQueue = new AudioQueue(environmentClipsFromDirectory, AudioQueueType.Environment);

            // Setup music audio
            AudioClip[] musicClipsFromDirectory = LoadAudioClipsFromDirectory(MusicPath); // Load audio clips from directory
            _musicAudioQueue = new AudioQueue(musicClipsFromDirectory, AudioQueueType.Music);

            // Setup storytelling audio
            _storytellingAudioSource = gameObject.AddComponent<AudioSource>(); // Create new audio source
            _storytellingAudioSource.playOnAwake = false;
            _storytellingAudioSource.outputAudioMixerGroup = storytellingMixerGroup; // Set storytelling mixer group
        }

        private void Start()
        {
            MasterVolumeSliderValue = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventPlayAudio, PlayStorytellingAudio);
            EventManager.StartListening(EventType.EventPauseAudio, StopStorytellingAudio);
            EventManager.StartListening(EventType.EventStateChange, SetDisplayInterface);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventPlayAudio, PlayStorytellingAudio);
            EventManager.StopListening(EventType.EventPauseAudio, StopStorytellingAudio);
            EventManager.StopListening(EventType.EventStateChange, SetDisplayInterface);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_setMasterVolumeCoroutine != null) StopCoroutine(_setMasterVolumeCoroutine);
            
            if (!hasFocus)
            {
                _setMasterVolumeCoroutine = StartCoroutine(SetMasterVolumeTarget(-80f));
            }
            else
            {
                _setMasterVolumeCoroutine = StartCoroutine(SetMasterVolumeTarget(0f));
            }
        }

        private void OnGUI()
        {
            if (!_displayOnGUI) return;
            if (!LockStateManager.IsPaused) return;
            
            // display a horizontal slider to control master volume at top right corner of screen
            GUI.Label(new Rect(Screen.width - 24 - MasterVolumeHorizontalSliderWidth, 8, MasterVolumeHorizontalSliderWidth, 100), "Master Volume");
            MasterVolumeSliderValue = GUI.HorizontalSlider(new Rect(Screen.width - 32 - MasterVolumeHorizontalSliderWidth, 32, MasterVolumeHorizontalSliderWidth, 24), MasterVolumeSliderValue, 0f, 1f);
        }

        #endregion

        #region AudioController specific methods

        /// <summary>
        /// Start playing audio from the environment audio queue if no environment audio is currently playing.
        /// </summary>
        public void StartEnvironmentLoop()
        {
            if (_environmentAudioQueue.Length <= 0)
            {
                Debug.LogWarning($"Environment audio queue is empty. Please add audio clips to {EnvironmentAudioPath} directory.");
                return;
            }
            _environmentCoroutine ??= StartCoroutine(AudioQueueCoroutine(_environmentAudioQueue)); // Start background audio loop
        }

        /// <summary>
        /// Stops playing audio from the environment audio queue.
        /// </summary>
        public void StopEnvironmentLoop()
        {
            if (_environmentCoroutine != null)
            {
                StopCoroutine(_environmentCoroutine);
                _environmentCoroutine = null;
            }
        }

        /// <summary>
        /// Start playing audio from the music audio queue if no music audio is currently playing.
        /// </summary>
        public void StartMusicLoop()
        {
            if (_musicAudioQueue.Length <= 0)
            {
                Debug.LogWarning($"Music audio queue is empty. Please add audio clips to {MusicPath} directory.");
                return;
            }
            _musicCoroutine ??= StartCoroutine(AudioQueueCoroutine(_musicAudioQueue));
        }

        /// <summary>
        /// Stops playing audio from the music audio queue.
        /// </summary>
        public void StopMusicLoop()
        {
            if (_musicCoroutine != null)
            {
                StopCoroutine(_musicCoroutine);
                _musicCoroutine = null;
            }
        }

        /// <summary>
        /// Add a new audio clip to an existing audio queue.
        /// </summary>
        /// <param name="clip">New AudioClip.</param>
        /// <param name="type">Select target queue type.</param>
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
                AudioClip audioClip = queue.GetNextAudioClipFromQueue(); // load next clip from queue

                Debug.Log($"[{nameof(AudioQueueCoroutine)}]: AudioClip: <color=#FF0000>{audioClip.name}</color> will be played next.");
                
                audioSource.clip = audioClip; // set loaded audio clip to audio source

                audioSource.Play(); // finally, play audio

                yield return new WaitUntil(() => audioSource.isPlaying == false); // wait until current audio clip finished playing
            }
        }

        private void PlayStorytellingAudio(EventParam eventParam)
        {
            var clip = eventParam.EventAudioClip;
            
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

        private void StopStorytellingAudio(EventParam eventParam)
        {
            if (_storytellingAudioSource.isPlaying)
                _storytellingAudioSource.Stop();
        }

        private IEnumerator SetMasterVolumeTarget(float volume)
        {
            masterMixerGroup.audioMixer.GetFloat("MasterVolume", out var currentVolume);
            float t = 0f;
            while (currentVolume != volume)
            {
                masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Lerp(currentVolume, volume, t));
                t += Time.deltaTime * .95f;
                yield return null;
            }
        }
        
        private void SetDisplayInterface(EventParam eventParam)
        {
            _displayOnGUI = eventParam.EventApplicationState == ApplicationState.Main;
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