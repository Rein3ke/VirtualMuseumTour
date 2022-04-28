using Controller;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Player
{
    /// <summary>
    /// This class is added as a component to the player prefab and controls the movement of the player.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementCharacterController : MonoBehaviour
    {
        #region Constants

        /// <summary>
        /// Gravity constant. Simulates the earth's gravity.
        /// </summary>
        private const float Gravity = -9.81f;

        #endregion

        #region Serialized Fields

        [SerializeField] private float playerSpeed = 4.0f;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;

        #endregion

        #region Members

        private CharacterController _characterController;
        private bool _isLocked;
        private Vector3 _velocity;

        #endregion

        #region Properties

        public static PlayerMovementCharacterController Instance { get; private set; }
        public bool IsGrounded { get; private set; }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Sets an instance of this class to prevent multiple player instances.
        /// </summary>
        private void Awake()
        {
            // set instance
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // destroy duplicate
                return;
            }
            Instance = this;

            // set components
            _characterController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Sets the internal lock state to "unlocked" so that the player can be controlled after initialization.
        /// </summary>
        private void Start()
        {
            _isLocked = false;
        }

        /// <summary>
        /// Moves the player GameObject according to the input.
        /// Enables the ability to jump if the player is on the ground.
        /// </summary>
        private void Update()
        {
            if (_isLocked || LockStateManager.IsPaused) return; // if the internal lock state is set to "locked" or the game is paused, the player is not controlled
            
            var currentTransform = transform; // get the current transform of the player
            IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // perform a sphere check to see if the player is on the ground
            
            // sets the velocity of the player to -2f until the player is grounded
            // this prevents the player from being able to jump while in the air
            if (IsGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        
            var x = Input.GetAxis("Horizontal"); // get the horizontal input ( A, D, Left, Right)
            var z = Input.GetAxis("Vertical"); // get the vertical input ( W, S, Up, Down)

            var movementVector = currentTransform.right * x + currentTransform.forward * z; // calculate the the motion vector by multiplying the right and the front vector of the current transformation with the input values

            _characterController.Move(movementVector * (playerSpeed * Time.deltaTime)); // move the player according to the input, player speed and delta time

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2 * Gravity);
            }

            _velocity.y += Gravity * Time.deltaTime; // apply gravity to the player over time so that he falls down

            _characterController.Move(_velocity * Time.deltaTime); // move the player according to the velocity
        }

        /// <summary>
        /// Subscribes to the LockControls event to lock or unlock the player movement when triggered.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLockControls, SetLockState);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, SetLockState);
        }

        /// <summary>
        /// Resets the instance to this class.
        /// </summary>
        private void OnDestroy()
        {
            if (Instance != null && Instance == this) Instance = null;
        }

        #endregion

        /// <summary>
        /// Sets the internal lock state when the SetLockState event is triggered.
        /// True = locked, false = unlocked.
        /// </summary>
        /// <param name="eventParam">Boolean value whether the player can be moved (via EventParam.EventBoolean).</param>
        private void SetLockState(EventParam eventParam)
        {
            _isLocked = eventParam.EventBoolean;
        }
    }
}
