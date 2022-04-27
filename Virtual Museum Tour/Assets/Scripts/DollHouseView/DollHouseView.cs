using System.Collections;
using System.Linq;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace DollHouseView
{
    /// <summary>
    /// Used to control the user input for the doll house view.
    /// Also manages all User Interface elements that are used for the camera target selection.
    /// </summary>
    public class DollHouseView : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Target that the camera should aim at.
        /// </summary>
        [Header("Target & Offset")]
        [SerializeField] private Transform target;

        /// <summary>
        /// Offset from the target position, so the camera is not directly in front of the target.
        /// </summary>
        [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
        
        /// <summary>
        /// Zoom speed.
        /// </summary>
        [Header("Camera Movement Settings")]
        [SerializeField] private float scrollSpeed = 10.0f;

        /// <summary>
        /// Rotation speed.
        /// </summary>
        [SerializeField] private float rotationSpeed = 10.0f;

        /// <summary>
        /// Displays the target's name in the UI.
        /// </summary>
        [Header("Interface")]
        [SerializeField] private TextMeshProUGUI targetText;
        /// <summary>
        /// Button to switch to another target.
        /// </summary>
        [SerializeField] private Button leftButton, rightButton;
        /// <summary>
        /// Button to disable the doll house view.
        /// </summary>
        [SerializeField] private Button backButton;
        
        #endregion

        #region Members

        /// <summary>
        /// The doll house view's camera.
        /// </summary>
        private Camera _camera;
        /// <summary>
        /// Is the doll house view currently active and controllable by the user?
        /// </summary>
        private bool _isControllable;
        /// <summary>
        /// All possible camera targets stored in an array.
        /// </summary>
        private GameObject[] _targetList;
        /// <summary>
        /// The index of the currently selected target in the array.
        /// </summary>
        private int _targetListIndex;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Get the camera component in the GameObject.
        /// </summary>
        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        /// <summary>
        /// Configures the camera and the UI.
        /// </summary>
        private void Start()
        {
            _camera.clearFlags = CameraClearFlags.Color; // Set the clear flag to only display the background color.
            _camera.backgroundColor = Color.black; // Background color is set to black.
            _camera.orthographic = false; // Set the camera to be a perspective camera.

            _isControllable = true; // Set the camera to be controllable as soon as the doll house view is enabled.

            // Add listeners to the buttons.
            leftButton.onClick.AddListener(GetPreviousTarget);
            rightButton.onClick.AddListener(GetNextTarget);
            backButton.onClick.AddListener(DisableDollHouseView);
        }

        /// <summary>
        /// Handles the camera rotation and target selection using the keyboard.
        /// </summary>
        private void Update()
        {
            if (!_isControllable) return;
            
            if (Input.GetKeyDown(KeyCode.R)) // R = Reset Camera Position
            {
                StartCoroutine(ResetPositionCoroutine());
            }

            if (Input.GetKeyDown(KeyCode.E)) // E = Set camera target to the next target in the array.
            {
                GetNextTarget();
            }
            if (Input.GetKeyDown(KeyCode.Q)) // Q = Set camera target to the previous target in the array.
            {
                GetPreviousTarget();
            }
            
            var movementVector = new Vector3();
        
            if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) movementVector += Vector3.left;
            if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) movementVector += Vector3.right;
            if (Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow)) movementVector += Vector3.down;
            if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow)) movementVector += Vector3.up;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                movementVector += Vector3.forward * scrollSpeed;
            } else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                movementVector += Vector3.back * scrollSpeed;
            }
        
            transform.LookAt(target); // Constantly look at the target.
            
            var calculatedRotationSpeed = rotationSpeed * (transform.position - target.position).magnitude; // Calculate the rotation speed based on the distance between the camera and the target.
            transform.Translate(movementVector * (calculatedRotationSpeed * Time.deltaTime), Space.Self); // Rotate the camera based on the movement vector.
        }

        /// <summary>
        /// Subscribes to multiple events.
        /// If a TeleportRequest event is received, call OnTeleportRequest() to disable the doll house view (the user wants to get teleported via POI to a selected PlayerSpawnPoint).
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventTeleportRequest, OnTeleportRequest);
            
            EventManager.TriggerEvent(EventType.EventDollHouseView, new EventParam // Trigger an event to notify other systems that the doll house view is enabled.
            {
                EventBoolean = true
            });
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam // Trigger an event to request control lock (prevents player movement).
            {
                EventBoolean = true
            });
            
            // Refresh the target array and set the first target as the current target.
            RefreshTargets();
            SetTarget(_targetList[0].transform);
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventTeleportRequest, OnTeleportRequest);
        }

        /// <summary>
        /// Removes all button listeners.
        /// </summary>
        private void OnDestroy()
        {
            leftButton.onClick.RemoveListener(GetPreviousTarget);
            rightButton.onClick.RemoveListener(GetNextTarget);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Listen to the TeleportRequest and disable the dollHouseView when invoked.
        /// </summary>
        private void OnTeleportRequest(EventParam eventParam)
        {
            DisableDollHouseView();
        }

        #endregion

        #region Target Management

        /// <summary>
        /// Finds all possible targets in the scene and stores them in the _targetList array.
        /// Targets have to be tagged as "Player" or "Exhibition".
        /// </summary>
        private void RefreshTargets()
        {
            var player = GameObject.FindGameObjectsWithTag("Player");
            var exhibitions = GameObject.FindGameObjectsWithTag("Exhibition");
            _targetList = player.Concat(exhibitions).ToArray();
            _targetListIndex = 0;
        }

        /// <summary>
        /// Receives a transform and sets it as the target.
        /// After setting the target, the camera will move to the target position using the ResetPositionCoroutine.
        /// </summary>
        /// <param name="newTarget">Target as transform.</param>
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
            
            var filteredTargetName = target.name.Replace("(Clone)", ""); // Filter "(Clone)" from the target name.
            targetText.text = $"{filteredTargetName} [{_targetListIndex}]";

            StartCoroutine(ResetPositionCoroutine());
        }

        /// <summary>
        /// Gets the next target in the _targetList array.
        /// If the current target is the last target in the array, the first target in the array will be set as the new target.
        /// </summary>
        private void GetNextTarget()
        {
            if (!_isControllable) return;
            
            if (_targetList == null || _targetList.Length <= 0) // Try to refresh the target list if it is set to null or empty.
            {
                RefreshTargets();

                if (_targetList.Length <= 0) // Return, if the target list is still empty.
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

        /// <summary>
        /// Gets the previous target in the _targetList array.
        /// If the current target is the first target in the array, the last target in the array will be set as the new target.
        /// </summary>
        private void GetPreviousTarget()
        {
            if (!_isControllable) return;
            
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

        #endregion

        #region Camera Movement

        /// <summary>
        /// Moves the camera back to the target position + offset over time.
        /// </summary>
        /// <returns>Nothing (Coroutine).</returns>
        private IEnumerator ResetPositionCoroutine()
        {
            if (target == null) yield break;
            
            Debug.Log("Start reset position coroutine!");
            
            _isControllable = false;

            var t = 0f;
            var currentPosition = transform.position;
            var targetPosition = target.position + offset;
            
            while (t < 1f) // Move the camera to the target position over time. Move and rotate the camera per frame according to the f-value.
            {
                var trans = transform;
                var lookDirection = (target.position - trans.position).normalized;
                transform.rotation = Quaternion.Slerp(trans.rotation, Quaternion.LookRotation(lookDirection), t);
                
                transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
                
                t += 0.75f * Time.deltaTime; // Determine speed of camera movement
                yield return null;
            }
            
            _isControllable = true;

            Debug.Log("Finished reset position coroutine!");
        }

        #endregion

        /// <summary>
        /// On disable, sets the DollHouseView to inactive and notifies other systems that the doll house view is disabled.
        /// Also unlocks the controls.
        /// </summary>
        private void DisableDollHouseView()
        {
            EventManager.TriggerEvent(EventType.EventDollHouseView, new EventParam
            {
                EventBoolean = false
            });
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam
            {
                EventBoolean = false
            });
            gameObject.SetActive(false);
        }
    }
}
