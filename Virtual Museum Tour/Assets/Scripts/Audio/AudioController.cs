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
    /// A controller that manages the AudioMixerGroups, the AudioQueues, and the playing and stopping of AudioClips.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        #region Constants

        /// <summary>
        /// Path to the AudioClips in the Resources folder which are used for everything related to the environment.
        /// </summary>
        private const string EnvironmentAudioPath = "AudioClips/Environment/";
        /// <summary>
        /// Path to the AudioClips in the Resources folder which are used for everything related to the music.
        /// </summary>
        private const string MusicPath = "AudioClips/Music/";
        /// <summary>
        /// Path to the AudioClips in the Resources folder which are used for everything related to the sound effects.
        /// </summary>
        private const string EffectsAudioPath = "AudioClips/Effects/";
        /// <summary>
        /// Path to the AudioClips in the Resources folder which are used for testing purposes.
        /// </summary>
        private const string TestAudioPath = "AudioClips/Test/";
        /// <summary>
        /// Width of the volume control GUI element.
        /// </summary>
        private const float MasterVolumeHorizontalSliderWidth = 96f;
        
        #endregion

        #region SerializeFields

        /// <summary>
        /// Master channel where all channels are merged.
        /// </summary>
        [SerializeField] private AudioMixerGroup masterMixerGroup;
        /// <summary>
        /// Channel for AudioSources that play audio clips related to the exhibit.
        /// </summary>
        [SerializeField] private AudioMixerGroup storytellingMixerGroup;
        /// <summary>
        /// Channel for AudioSources, which play ambient audio clips.
        /// </summary>
        [SerializeField] private AudioMixerGroup environmentMixerGroup;
        /// <summary>
        /// Channel for AudioSources, which play music audio clips.
        /// </summary>
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        #endregion

        #region Members

        /// <summary>
        /// AudioSource, which is used for all exhibit relevant audio clips.
        /// </summary>
        private AudioSource _storytellingAudioSource;
        /// <summary>
        /// AudioQueue, which contains all environment-relevant audio clips.
        /// </summary>
        private AudioQueue _environmentAudioQueue;
        /// <summary>
        /// AudioQueue, which contains all music-relevant audio clips.
        /// </summary>
        private AudioQueue _musicAudioQueue;
        
        /// <summary>
        /// Coroutine or process that is executed asynchronously to play ambient audio clips.
        /// </summary>
        private Coroutine _environmentCoroutine;
        /// <summary>
        /// Coroutine or process that is executed asynchronously to play music audio clips.
        /// </summary>
        private Coroutine _musicCoroutine;
        /// <summary>
        /// Coroutine that smoothly changes the volume of the master channel.
        /// </summary>
        private Coroutine _setMasterVolumeCoroutine;
        
        /// <summary>
        /// Current value of the master volume slider.
        /// </summary>
        private float _masterVolumeSliderValue;
        /// <summary>
        /// Boolean specifying whether the GUI interface should be displayed.
        /// </summary>
        private bool _displayOnGUI;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the current value of the main volume slider.
        /// When set, the slider value is updated, as well as the audio listener volume and the value in the player presets. 
        /// </summary>
        private float MasterVolumeSliderValue
        {
            get => _masterVolumeSliderValue;
            set
            {
                _masterVolumeSliderValue = value;
                AudioListener.volume = value;
                PlayerPrefs.SetFloat("MasterVolume", value);
            }
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Loads all audio clips from the resources folder and adds them to their corresponding AudioQueues.
        /// Also adds an AudioSource to the GameObject to play exhibit-related audio.
        /// </summary>
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

        /// <summary>
        /// Loads the master volume from the player presets.
        /// </summary>
        private void Start()
        {
            MasterVolumeSliderValue = PlayerPrefs.GetFloat("MasterVolume", 1f); // Update slider based on the value stored in player presets. Default value is 1 (max volume).
        }

        /// <summary>
        /// Subscribes to multiple events to handle audio playback requests and whether the GUI interface should be displayed.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventPlayAudio, PlayStorytellingAudio); // Play audio from the selected audio clip dropdown
            EventManager.StartListening(EventType.EventPauseAudio, StopStorytellingAudio); // Stop audio playback
            EventManager.StartListening(EventType.EventStateChange, SetDisplayInterface); // Toggle the GUI interface
        }

        /// <summary>
        /// Unsubscribes from multiple events.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventPlayAudio, PlayStorytellingAudio);
            EventManager.StopListening(EventType.EventPauseAudio, StopStorytellingAudio);
            EventManager.StopListening(EventType.EventStateChange, SetDisplayInterface);
        }

        /// <summary>
        /// Uses the SetMasterVolumeCoroutine to adjust the volume.
        /// Triggered depending on whether the application is focused or not.
        /// Example: When the browser tab is switched, the application is no longer focused and the volume is set to zero.
        /// </summary>
        /// <param name="hasFocus">True, if the application is focused.</param>
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

        /// <summary>
        /// Draws a slider in the corner of the screen to adjust the volume.
        /// </summary>
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
        /// Adds a new audio clip based on the queue type to an existing audio queue.
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
        /// Coroutine to manage the asynchronous playback of AudioClips via an AudioQueue.
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

        /// <summary>
        /// Callback method to enable playback of an AudioClip passed in the event.
        /// </summary>
        /// <param name="eventParam">AudioClip (via eventParam.EventAudioClip).</param>
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

            #if UNITY_EDITOR // if in editor, play test audio instead of actual audio
                _storytellingAudioSource.clip = Resources.LoadAll<AudioClip>(TestAudioPath)[0];
            #else
                _storytellingAudioSource.clip = clip;
            #endif
            
            _storytellingAudioSource.Play();
        }

        /// <summary>
        /// Callback method to stop playing an AudioClip via the Storytelling AudioSource.
        /// </summary>
        /// <param name="eventParam">(Obsolete).</param>
        private void StopStorytellingAudio(EventParam eventParam)
        {
            if (_storytellingAudioSource.isPlaying)
                _storytellingAudioSource.Stop();
        }

        /// <summary>
        /// Coroutine to adjust the master volume to a specified value over time.
        /// </summary>
        /// <param name="volume">Desired volume.</param>
        /// <returns>Nothing (Coroutine).</returns>
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
        
        /// <summary>
        /// Callback method that allows displaying GUI elements as soon as the application is in main state.
        /// </summary>
        /// <param name="eventParam">Application state (via eventParam.EventApplicationState).</param>
        private void SetDisplayInterface(EventParam eventParam)
        {
            _displayOnGUI = eventParam.EventApplicationState == ApplicationState.Main;
        }

        #endregion

        #region Resource Management

        /// <summary>
        /// Returns an array of audio clips found at the passed path in the resource folder.
        /// </summary>
        /// <param name="path">Path to the AudioClips in the Resources folder.</param>
        /// <returns>Array of found AudioClips.</returns>
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