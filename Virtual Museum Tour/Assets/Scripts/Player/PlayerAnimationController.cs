using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAnimationController : MonoBehaviour
    {
        // constants
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsMovingBool = Animator.StringToHash("IsMovingBool");
        
        // members
        private PlayerMovementCharacterController _characterController;
        private Transform _playerTransform;
        private Vector3 _lastPosition = Vector3.zero;
        
        private AudioSource _audioSource;

        private Animator _animationController;

        private void Awake()
        {
            _characterController = PlayerMovementCharacterController.Instance;
            _playerTransform = _characterController.transform;
            
            _audioSource = GetComponent<AudioSource>();
            
            _animationController = GetComponent<Animator>();
        }

        private void Start()
        {
            _animationController.ResetTrigger(IsMoving);
        }

        private void FixedUpdate()
        {
            if (_playerTransform.position != _lastPosition)
            {
                // player has moved
                _animationController.SetBool(IsMovingBool, true);
            }
            else
            {
                // player hasn't moved
                _animationController.SetBool(IsMovingBool, false);
            }

            _lastPosition = _playerTransform.position;
        }

        // ReSharper disable once UnusedMember.Global
        // Method used in animation event
        public void PlayFootstep()
        {
            var random = Random.Range(0.75f, 1.25f);
            _audioSource.pitch = random;
            _audioSource.Play();
        }
    }
}
