using UnityEngine;

public class PlayerMovementCharacterController : MonoBehaviour
{
    private CharacterController _characterController;
    
    [SerializeField] private float playerSpeed = 4.0f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpHeight = 3f;

    private const float Gravity = -9.81f;

    private Vector3 _velocity;
    private bool _isGrounded;
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {   
        var currentTransform = transform;
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = currentTransform.right * x + currentTransform.forward * z;

        _characterController.Move(move * playerSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2 * Gravity);
        }

        _velocity.y += Gravity * Time.deltaTime;

        _characterController.Move(_velocity * Time.deltaTime);
    }
}
