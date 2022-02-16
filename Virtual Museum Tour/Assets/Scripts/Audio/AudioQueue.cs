using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Audio
{
    public class AudioQueue
    {
        public AudioQueueType QueueType { get; private set; }
        private readonly List<AudioClip> _audioClips;
        
        public int Length => _audioClips.Count;
        
        public AudioQueue(AudioQueueType queueType)
        {
            this.QueueType = queueType;
            _audioClips = new List<AudioClip>();
        }

        public AudioQueue([NotNull] List<AudioClip> audioClips, AudioQueueType queueType)
        {
            _audioClips = audioClips;
            this.QueueType = queueType;
        }

        public AudioQueue([NotNull] AudioClip[] audioClips, AudioQueueType queueType)
        {
            this.QueueType = queueType;
            _audioClips = new List<AudioClip>();
            _audioClips.AddRange(audioClips);
        }

        public void Add([NotNull] AudioClip audioClip)
        {
            _audioClips.Add(audioClip);
        }
        
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