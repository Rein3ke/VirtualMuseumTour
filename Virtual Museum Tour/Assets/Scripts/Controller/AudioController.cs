using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Controller
{
    /// <summary>
    /// An controller that handles all the audio clips which should be played.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        private const string AudioPath = "AudioClips/";
        
        public static AudioController Instance { get; private set; }

        private AudioSource _audioSource;
        private List<AudioClip> _audioClipQueue;
        private AudioClip _currentAudioClip;
        private Coroutine _audioPlayRoutine;

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
            _audioClipQueue = new List<AudioClip>();
            
            // Audio Source Configuration
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            
            // If ready, play test sounds
            _audioClipQueue.Add((AudioClip) Resources.Load($"{AudioPath}ambient_environment_museum_02"));
            _audioClipQueue.Add((AudioClip) Resources.Load($"{AudioPath}ambient_environment_museum_01"));
        }

        private void Update()
        {
            if (_audioSource.isPlaying || _audioClipQueue.Count <= 0 || _audioPlayRoutine != null) return;

            _audioPlayRoutine = StartCoroutine(PlayAudioRoutine(GetNextAudioClipFromQueue()));
        }

        private void OnDestroy()
        {
            if (Instance != null && Instance == this) Instance = null;
        }

        #endregion
        
        #region AudioController specific methods

        public void PlayAudioClip([NotNull] AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("AudioClip is null.");
                return;
            }
            
            if (_audioSource == null || _audioSource.isPlaying) 
            {
                _audioClipQueue.Add(clip);
                Debug.Log($"AudioClip {clip.name} was added to queue!");
                return;
            }
            
            _audioPlayRoutine = StartCoroutine(PlayAudioRoutine(clip));
        }

        private IEnumerator PlayAudioRoutine([NotNull] AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("AudioClip is null.", this);
                yield return null;
            }

            // Load clip if isn't loaded yet.
            if (clip.loadInBackground)
            {
                clip.LoadAudioData();
                yield return new WaitUntil(() => clip.loadState == AudioDataLoadState.Loaded);
                Debug.Log($"AudioClip {clip} loaded in background.");
            }

            _currentAudioClip = clip;

            // Load clip in audio source and play it
            _audioSource.clip = _currentAudioClip;
            _audioSource.Play();
            yield return new WaitUntil(() => !_audioSource.isPlaying);
            _audioPlayRoutine = null;
            Debug.Log("Finished playing audio.");
        }
        
        private AudioClip GetNextAudioClipFromQueue()
        {
            Debug.Log("Get next audio clip from queue!");
            
            var nextAudioClip = _audioClipQueue.FirstOrDefault();
            // If nextAudioClip could get loaded, remove it from the queue
            if (nextAudioClip != null)
            {
                _audioClipQueue.RemoveAt(0);
                return nextAudioClip;
            }

            return null;
        }
        
        #endregion
    }
}
