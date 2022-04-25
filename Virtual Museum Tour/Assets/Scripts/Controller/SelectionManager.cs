using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Controller
{
    /// <summary>
    /// Uses ray casting to detect if the player is looking at an interactable object (e.g. Exhibit).
    /// </summary>
    public class SelectionManager : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// The raycast layer mask on which interactable objects are to be detected.
        /// </summary>
        [SerializeField] private LayerMask raycastLayerMask;
        /// <summary>
        /// The maximum distance at which the raycast is to be cast.
        /// </summary>
        [SerializeField] private float maxRaycastDistance = 10f;

        #endregion

        #region Members

        /// <summary>
        /// Stores the transform of the currently selected interactable object.
        /// </summary>
        private Transform _currentlySelectedGameObject;
        /// <summary>
        /// Defines whether a raycast can be cast or not.
        /// If true, the process of ray casting is not allowed.
        /// </summary>
        private bool _isLocked;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Enable the raycast process on start.
        /// </summary>
        private void Start()
        {
            _isLocked = false;
        }

        /// <summary>
        /// Performs a RayCast using the player camera. When something is detected, save the selected object.
        /// Triggers an event when an object is selected and the user left clicks.
        /// </summary>
        private void Update()
        {
            if (_isLocked) return;
            
            // 1. get the player camera
            var originCamera = GameObject.FindGameObjectWithTag("PlayerCamera")?.GetComponent<Camera>();
            if (originCamera == null) return; // if no camera is found, return

            // 2. clear the previously selected object
            if (_currentlySelectedGameObject != null)
            {
                _currentlySelectedGameObject = null;
            }
            
            // 3. perform a ray cast from camera to screen
            var ray = originCamera.ScreenPointToRay(Input.mousePosition);

            // 4. handle raycast hit
            if (Physics.Raycast(ray, out var hit, maxRaycastDistance, raycastLayerMask))
            {
                var selectionTransform = hit.transform;
                if (selectionTransform.CompareTag("Exhibit")) // check, if the object is an exhibit
                {
                    MouseCursorController.SetCursorTexture(MouseCursorController.SearchTexture);

                    _currentlySelectedGameObject = selectionTransform;
                }
            }
            else
            {
                MouseCursorController.SetCursorTexture(null);
            }

            // (5.) do something if something is selected and left mouse button is pressed
            if (Input.GetMouseButtonDown(0) && _currentlySelectedGameObject != null)
            {
                // Find anchor of selection and invoke event
                if (HasExhibitAnchor(_currentlySelectedGameObject.gameObject, out var anchor))
                {
                    ExhibitManager.ExhibitDictionary.TryGetValue(anchor.GetComponent<ExhibitAnchor>().ExhibitID, out var exhibit);

                    EventManager.TriggerEvent(EventType.EventExhibitSelect, new EventParam
                    {
                        EventExhibit = exhibit
                    });
                }
            }
        }

        /// <summary>
        /// Subscribes to the EventLockControls event and calls SetLockState.
        /// EventLockControls is called when the ray casting should be disabled.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventLockControls, SetLockState);
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventLockControls, SetLockState);
        }

        #endregion

        /// <summary>
        /// Returns true if the given game object has an corresponding exhibit anchor.
        /// Stores the anchor in the given out parameter so that it can be used.
        /// </summary>
        /// <param name="selection">Selected exhibit asset in scene.</param>
        /// <param name="anchor">ExhibitAnchor of the selected exhibit.</param>
        /// <returns>True, if the given game object has an corresponding exhibit anchor.</returns>
        private bool HasExhibitAnchor(GameObject selection, out GameObject anchor)
        {
            GameObject current = selection;
            
            do // iterate through the parents of the given GameObject until an anchor is found
            {
                if (current.CompareTag("ExhibitAnchor")) // Return true, if the current GameObject is an anchor
                {
                    anchor = current;
                    return true;
                }

                current = current.transform.parent.gameObject; // If not, go one level up in the hierarchy
                
            } while (current.transform.parent != null); // Stop, if the current GameObject has no parent

            // If no anchor was found, return false and set anchor to null
            anchor = null;
            return false;
        }

        #region Event Handlers

        /// <summary>
        /// Gets called when the ray casting should be enabled or disabled.
        /// </summary>
        /// <param name="eventParam">Boolean (true = enable RayCasting, false = disable RayCasting) (via eventParam.EventBoolean).</param>
        private void SetLockState(EventParam eventParam)
        {
            _isLocked = eventParam.EventBoolean;
        }

        #endregion
    }
}