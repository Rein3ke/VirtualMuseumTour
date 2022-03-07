using System.Collections;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace DollHouseView
{
    public class DollHouseView : MonoBehaviour
    {
        [Header("Target & Offset")]
        [SerializeField] private Transform target;

        [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

        [Header("Camera Movement Settings")]
        [SerializeField] private float scrollSpeed = 10.0f;

        [SerializeField] private float rotationSpeed = 10.0f;

        private Camera _camera;
        private bool _isControllable;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            _camera.clearFlags = CameraClearFlags.Color;
            _camera.backgroundColor = Color.black;
            _camera.orthographic = false;

            _isControllable = true;
            
            GetNewTarget();
            ResetPosition();
        }

        private void Update()
        {
            if (!_isControllable) return;
            
            if (Input.GetKeyDown(KeyCode.R)) // R = Reset Camera Position
            {
                StartCoroutine(ResetPositionCoroutine());
            }
            if (Input.GetMouseButtonDown(1)) // Right Click = Disable camera
            {
                gameObject.SetActive(false);
                EventManager.TriggerEvent(EventType.EventOpenDollHouseView, new EventParam
                {
                    EventBoolean = false
                });
            }
            
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

        private IEnumerator ResetPositionCoroutine()
        {
            if (target == null) yield break;
            
            Debug.Log("Start reset position coroutine!");
            
            _isControllable = false;

            var t = 0f;
            var currentPosition = transform.position;
            var targetPosition = target.position + offset;
            
            while (t < 1f)
            {
                var trans = transform;
                var lookDirection = (target.position - trans.position).normalized;
                transform.rotation = Quaternion.Slerp(trans.rotation, Quaternion.LookRotation(lookDirection), t);
                
                transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
                
                t += 0.75f * Time.deltaTime;
                Debug.Log($"{t}");
                yield return null;
            }
            
            _isControllable = true;

            Debug.Log("Finished reset position coroutine!");
        }

        public void SetTarget(Transform newTarget)
        {
            if (newTarget == null || target == newTarget)
            {
                Debug.LogError($"Target {newTarget} is either null or already set.");
                return;
            }
            
            target = newTarget;
            ResetPosition();
        }

        private void ResetPosition()
        {
            if (target == null)
            {
                Debug.LogError($"Can't reset position of {nameof(DollHouseView)}-Camera because target is null.");
                return;
            }
            
            transform.position = target.position + offset;
        }

        private void GetNewTarget()
        {
            var exhibition = GameObject.FindGameObjectWithTag("Exhibition");

            if (exhibition != null)
            {
                SetTarget(exhibition.transform);
            }
        }
    }
}
