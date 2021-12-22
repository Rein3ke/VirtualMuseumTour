using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Controller
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }
        
        [SerializeField] private Material highlightMaterial;

        public event EventHandler<OnExhibitSelectedEventArgs> OnExhibitSelected;

        private Transform _selection;
        private Material _selectionMaterial;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            // 1. check for main camera
            var mainCamera = Camera.main;
            if (mainCamera == null) return;

            // 2. if something is selected, reset
            if (_selection != null)
            {
                var selectionRenderer = _selection.GetComponent<Renderer>();
                selectionRenderer.material = _selectionMaterial;
                _selection = null;
            }

            // 3. cast ray from camera to screen
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 4. handle raycast hit
            if (Physics.Raycast(ray, out var hit))
            {
                var selection = hit.transform;
                if (selection.CompareTag("Exhibit"))
                {
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        _selectionMaterial = selectionRenderer.material;
                        selectionRenderer.material = highlightMaterial;
                    }

                    _selection = selection;
                }
            }

            // (5.) do something if something is selected and left mouse button is pressed
            if (Input.GetMouseButtonDown(0) && _selection != null)
            {
                // Find anchor of selection and invoke event
                if (HasExhibitAnchor(_selection.gameObject, out var anchor))
                {
                    
                    ExhibitManager.Instance.ExhibitDictionary.TryGetValue(anchor.GetComponent<ExhibitAnchor>().ExhibitID, out var exhibit);

                    InvokeOnExhibitSelected(exhibit);
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
        
        #region Events

        private void InvokeOnExhibitSelected(Exhibit exhibit)
        {
            if (exhibit == null)
            {
                Debug.LogError("Given exhibit is null!", this);
                return;
            }
            OnExhibitSelected?.Invoke(this, new OnExhibitSelectedEventArgs(exhibit));
        }

        #endregion
    }

    public class OnExhibitSelectedEventArgs: EventArgs
    {
        public Exhibit Exhibit { get; private set; }

        public OnExhibitSelectedEventArgs(Exhibit exhibit)
        {
            Exhibit = exhibit;
        }
    }
}