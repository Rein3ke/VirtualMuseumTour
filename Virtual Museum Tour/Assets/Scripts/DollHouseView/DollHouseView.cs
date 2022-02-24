using UnityEngine;

namespace DollHouseView
{
    public class DollHouseView : MonoBehaviour
    {
        [Header("DollHouseView")]
        [SerializeField] private Transform target;
        [SerializeField] private float scrollSpeed = 10.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        private Camera _camera;
    
        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            _camera.clearFlags = CameraClearFlags.Color;
            _camera.backgroundColor = Color.black;
            _camera.orthographic = false;
        }

        private void Update()
        {
            var movementVector = new Vector3();
        
            if (Input.GetKey(KeyCode.A)) movementVector += Vector3.left;
            if (Input.GetKey(KeyCode.D)) movementVector += Vector3.right;
            if (Input.GetKey(KeyCode.S)) movementVector += Vector3.down;
            if (Input.GetKey(KeyCode.W)) movementVector += Vector3.up;
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                movementVector += Vector3.forward * scrollSpeed;
            } else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                movementVector += Vector3.back * scrollSpeed;
            }
        
            transform.LookAt(target);
            transform.Translate(movementVector * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
