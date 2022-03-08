using System;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementCharacterController : MonoBehaviour
    {
        #region Constants

        private const float Gravity = -9.81f;

        #endregion
        
        #region SerializeFields

        [SerializeField] private float playerSpeed = 4.0f;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;

        #endregion

        #region Members

        private CharacterController _characterController;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isLocked;

        #endregion

        public static PlayerMovementCharacterController Instance { get; private set; }

        private void Awake()
        {
            // set instance
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // set components
            _characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _isLocked = false;
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLockControls, SetLockState);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, SetLockState);
        }

        private void SetLockState(EventParam eventParam)
        {
            _isLocked = eventParam.EventBoolean;
        }

        private void Update()
        {
            if (_isLocked) return;
            
            var currentTransform = transform;
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = currentTransform.right * x + currentTransform.forward * z;

            _characterController.Move(move * (playerSpeed * Time.deltaTime));

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2 * Gravity);
            }

            _velocity.y += Gravity * Time.deltaTime;

            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (Instance != null && Instance == this) Instance = null;
        }
    }
}
