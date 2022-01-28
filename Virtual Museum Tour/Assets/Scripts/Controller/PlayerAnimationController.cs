using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayFootstep()
        {
            _audioSource.Play();
        }
    }
}
