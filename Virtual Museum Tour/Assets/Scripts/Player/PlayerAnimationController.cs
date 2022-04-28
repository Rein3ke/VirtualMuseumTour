using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    /// <summary>
    /// This class is added to the player prefab as a component and handles the Head bobbing during movement.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAnimationController : MonoBehaviour
    {
        #region Constants

        /// <summary>
        /// Stores a variable used in the animator as a hash. (Hashes are used for optimized setters and getters on parameters).
        /// </summary>
        private static readonly int IsMovingBool = Animator.StringToHash("IsMovingBool");

        #endregion

        #region Members

        /// <summary>
        /// Stores a reference to the PlayerMovementCharacterController of the player GameObject.
        /// </summary>
        private PlayerMovementCharacterController _characterController;
        /// <summary>
        /// Stores the transform of the player GameObject.
        /// </summary>
        private Transform _playerTransform;
        /// <summary>
        /// Stores the last position of the player in a Vector3.
        /// </summary>
        private Vector3 _lastPosition = Vector3.zero;
        /// <summary>
        /// Stores a AudioSource component to play footstep sounds.
        /// </summary>
        private AudioSource _audioSource;
        /// <summary>
        /// Stores a reference to the animator component of the player GameObject.
        /// </summary>
        private Animator _animationController;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Stores all necessary references in the member variables.
        /// </summary>
        private void Awake()
        {
            _characterController = PlayerMovementCharacterController.Instance;
            _playerTransform = _characterController.transform;
            _audioSource = GetComponent<AudioSource>();
            _animationController = GetComponent<Animator>();
        }

        /// <summary>
        /// On each frame, the player is checked if he is moving. If he is, the animators IsMovingBool parameter is set to true.
        /// FixedUpdate is used for physics calculations.
        /// </summary>
        private void FixedUpdate()
        {
            // check if player is moving
            if (_playerTransform.position != _lastPosition) // player has moved
            {
                _animationController.SetBool(IsMovingBool, true);
            }
            else // player hasn't moved
            {
                _animationController.SetBool(IsMovingBool, false);
            }

            _lastPosition = _playerTransform.position; // store last position
        }

        #endregion

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Plays a footstep sound with a random pitch to simulate the player walking.
        /// This method is called directly from the footstep animation.
        /// </summary>
        public void PlayFootstep()
        {
            if (!_characterController.IsGrounded) return; // only play footstep sound if player is on the ground
            
            var random = Random.Range(0.75f, 1.25f); // choose a random pitch
            _audioSource.pitch = random; // set the pitch
            _audioSource.Play();
        }
    }
}
