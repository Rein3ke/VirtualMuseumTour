using System;
using System.Collections;
using Events;
using TMPro;
using UnityEngine;
using EventType = Events.EventType;
using Random = UnityEngine.Random;

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

        [Header("Interface")]
        [SerializeField] private TextMeshProUGUI targetText;

        private Camera _camera;
        private bool _isControllable;
        private GameObject[] _targetList;
        private int _targetListIndex;

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
            
            GetRandomTarget();
        }

        private void OnEnable()
        {
            EventManager.TriggerEvent(EventType.EventOpenDollHouseView, new EventParam
            {
                EventBoolean = true
            });
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam
            {
                EventBoolean = true
            });
        }

        private void OnDisable()
        {
            EventManager.TriggerEvent(EventType.EventOpenDollHouseView, new EventParam
            {
                EventBoolean = false
            });
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam
            {
                EventBoolean = false
            });
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
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                GetNextTarget();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GetPreviousTarget();
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
            transform.Translate(movementVector * (rotationSpeed * Time.deltaTime), Space.Self);
        }

        private void RefreshTargets()
        {
            _targetList = GameObject.FindGameObjectsWithTag("Exhibition");
            _targetListIndex = 0;
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

        private void SetTarget(Transform newTarget)
        {
            if (newTarget == null)
            {
                Debug.LogError($"Target {newTarget} is null.");
                return;
            }

            if (target == newTarget)
            {
                Debug.LogWarning($"Target {newTarget} is already set.");
                return;
            }
            
            target = newTarget;
            targetText.text = $"Selected target: {target.name} [{_targetListIndex}]";

            StartCoroutine(ResetPositionCoroutine());
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

        private void GetRandomTarget()
        {
            RefreshTargets();
            
            if (_targetList.Length <= 0)
            {
                Debug.LogWarning("No targets found!");
                return;
            }
            
            _targetListIndex = Random.Range(0, _targetList.Length);
            SetTarget(_targetList[_targetListIndex].transform);
        }

        private void GetNextTarget()
        {
            if (_targetList == null || _targetList.Length <= 0)
            {
                RefreshTargets();

                if (_targetList.Length <= 0)
                {
                    return;
                }
            }

            _targetListIndex++;
            
            if (_targetListIndex > _targetList.Length - 1)
            {
                _targetListIndex = 0;
            }
            
            SetTarget(_targetList[_targetListIndex].transform);
        }

        private void GetPreviousTarget()
        {
            if (_targetList == null || _targetList.Length <= 0)
            {
                RefreshTargets();

                if (_targetList.Length <= 0)
                {
                    return;
                }
            }
            
            _targetListIndex--;
            
            if (_targetListIndex < 0)
            {
                _targetListIndex = (_targetList.Length - 1);
            }
            
            SetTarget(_targetList[_targetListIndex].transform);
        }
    }
}
