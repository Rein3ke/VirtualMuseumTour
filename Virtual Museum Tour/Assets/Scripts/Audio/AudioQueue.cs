using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Stores audio clips and has logic to output audio clips and place them at the end of a list to simulate a queue.
    /// </summary>
    public class AudioQueue
    {
        /// <summary>
        /// Returns the type of the AudioQueue.
        /// </summary>
        public AudioQueueType QueueType { get; private set; }

        /// <summary>
        /// Returns the number of all stored audio clips.
        /// </summary>
        public int Length => _audioClips.Count;

        /// <summary>
        /// A list to store audio clips.
        /// </summary>
        private readonly List<AudioClip> _audioClips;

        /// <summary>
        /// Constructor: Initializes the AudioQueue with the given type.
        /// </summary>
        /// <param name="queueType">AudioQueue Type (e.g. Environment).</param>
        public AudioQueue(AudioQueueType queueType)
        {
            this.QueueType = queueType;
            _audioClips = new List<AudioClip>();
        }

        /// <summary>
        /// Constructor: Initializes the AudioQueue with the given type and audio clips.
        /// </summary>
        /// <param name="audioClips">List of AudioClips.</param>
        /// <param name="queueType">AudioQueue Type (e.g. Environment).</param>
        public AudioQueue([NotNull] List<AudioClip> audioClips, AudioQueueType queueType)
        {
            _audioClips = audioClips;
            this.QueueType = queueType;
        }

        /// <summary>
        /// Constructor: Initializes the AudioQueue with the given type and audio clips.
        /// </summary>
        /// <param name="audioClips">Array of AudioClips.</param>
        /// <param name="queueType">AudioQueue Type (e.g. Environment).</param>
        public AudioQueue([NotNull] AudioClip[] audioClips, AudioQueueType queueType)
        {
            this.QueueType = queueType;
            _audioClips = new List<AudioClip>();
            _audioClips.AddRange(audioClips);
        }

        /// <summary>
        /// Adds an AudioClip to the end of the list.
        /// </summary>
        /// <param name="audioClip">New AudioClip.</param>
        public void Add([NotNull] AudioClip audioClip)
        {
            _audioClips.Add(audioClip);
        }
        
        /// <summary>
        /// Returns the first AudioClip in the list and adds it back to the end if RemoveAudioClip is set to false.
        /// </summary>
        /// <param name="removeAudioClip">Should the AudioClip be deleted afterwards?</param>
        /// <returns>First AudioClip from the list.</returns>
        public AudioClip GetNextAudioClipFromQueue(bool removeAudioClip = false)
        {
            if (_audioClips.Count == 0) return null;

            var firstEntry = _audioClips[0];
            if (removeAudioClip)
            {
                _audioClips.RemoveAt(0);
            }
            else
            {
                _audioClips.RemoveAt(0);
                Add(firstEntry);
            }

            Debug.Log($"[{nameof(GetNextAudioClipFromQueue)}] Audio clips left in queue: {_audioClips.Count}");
            
            return firstEntry;
        }
    }
}