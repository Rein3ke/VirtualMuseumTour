using UnityEngine;

namespace Controller
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private Material highlightMaterial;

        private Transform _selection;
        private Material _selectionMaterial;
        
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
                Debug.Log($"Selected: {_selection.name}");
            }
        }
    }
}
