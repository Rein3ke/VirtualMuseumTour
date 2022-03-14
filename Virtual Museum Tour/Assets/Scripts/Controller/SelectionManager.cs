using System;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace Controller
{
    public class SelectionManager : MonoBehaviour
    {
        [Header("Highlight")]
        [SerializeField] private Material highlightMaterial;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private float maxRaycastDistance = 10f;
        
        private Transform _currentlySelectedGameObject;
        private Material _selectionMaterial;
        private bool _isLocked;

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
            
            // 1. check for player camera
            var originCamera = GameObject.FindGameObjectWithTag("PlayerCamera")?.GetComponent<Camera>();
            if (originCamera == null) return;

            // 2. if something is selected, reset
            if (_currentlySelectedGameObject != null)
            {
                /*var selectionRenderer = _currentlySelectedGameObject.GetComponent<Renderer>();
                selectionRenderer.material = _selectionMaterial;*/
                _currentlySelectedGameObject = null;
            }
            
            // 3. cast ray from camera to screen
            var ray = originCamera.ScreenPointToRay(Input.mousePosition);

            // 4. handle raycast hit
            if (Physics.Raycast(ray, out var hit, maxRaycastDistance, raycastLayerMask))
            {
                var selectionTransform = hit.transform;
                if (selectionTransform.CompareTag("Exhibit"))
                {
                    var selectionRenderer = selectionTransform.GetComponent<Renderer>();
                    /*if (selectionRenderer != null)
                    {
                        _selectionMaterial = selectionRenderer.material;
                        selectionRenderer.material = highlightMaterial;
                    }*/
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

        private bool HasExhibitAnchor(GameObject selection, out GameObject anchor)
        {
            GameObject current = selection;
            
            do
            {
                if (current.CompareTag("ExhibitAnchor"))
                {
                    anchor = current;
                    return true;
                }

                current = current.transform.parent.gameObject;
                
            } while (current.transform.parent != null);

            anchor = null;
            return false;
        }
    }
}