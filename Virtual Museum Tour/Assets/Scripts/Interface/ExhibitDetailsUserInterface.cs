using System;
using Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class ExhibitDetailsUserInterface : MonoBehaviour, IUserInterface
    {
        public static ExhibitDetailsUserInterface Instance { get; private set; }

        [SerializeField] private float scaleFactor = 100f;
        [SerializeField] private GameObject modelHolder;
        [SerializeField] private Camera interfaceCamera;
        [SerializeField] private Text title;
        [SerializeField] private Text description;

        public event EventHandler<OnVisibilityChangeEventArgs> OnVisibilityChange;

        private Exhibit _currentExhibit;
        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                InvokeOnVisibilityChange(_isVisible);
            }
        }

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

        private void Start() {
            // 1. hide interface at start
            HideInterface();
            
            // 2. subscribe from events
            SelectionManager.Instance.OnExhibitSelected += OnExhibitSelected;
        }

        private void Update()
        {
            if (!_isVisible) return;

            if (Input.GetMouseButtonDown(1))
            {
                HideInterface();
            }
        }

        public void ShowInterface()
        {
            interfaceCamera.enabled = true;
            IsVisible = true;
        }

        public void HideInterface()
        {
            interfaceCamera.enabled = false;
            IsVisible = false;
        }
        
        private GameObject GetAttachedModel()
        {
            var attachedModel = modelHolder.transform.GetChild(0).gameObject;

            return attachedModel != null ? attachedModel : null;
        }

        private static void AttachScriptToModel(GameObject model)
        {
            model.AddComponent<MouseDragRotate>();
        }

        private void InterfaceSetup(Exhibit exhibit)
        {
            // set variables
            title.text = exhibit.Name;
            description.text = exhibit.Description;
            // set model
            Destroy(modelHolder.transform.GetChild(0).gameObject);
            var attachedGameObject = Instantiate(exhibit.Asset.GetComponentInChildren<Renderer>().gameObject, modelHolder.transform);
            attachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            attachedGameObject.layer = LayerMask.NameToLayer("UI");
            // attach drag and rotation script to model
            AttachScriptToModel(attachedGameObject);
        }

        #region Events

        public void InvokeOnVisibilityChange(bool isVisible)
        {
            OnVisibilityChange?.Invoke(this, new OnVisibilityChangeEventArgs(isVisible));
        }

        #endregion
        
        #region Event Handling

        private void OnExhibitSelected(object sender, OnExhibitSelectedEventArgs e)
        {
            _currentExhibit = e.Exhibit;
            InterfaceSetup(_currentExhibit);
            ShowInterface();
        }

        #endregion

        private void OnDestroy()
        {
            // unsubscribe from events
            SelectionManager.Instance.OnExhibitSelected -= OnExhibitSelected;
            // reset instance
            if (Instance != null && Instance == this)
            {
                Instance = null;
            }
        }
    }

    public class OnVisibilityChangeEventArgs : EventArgs
    {
        public bool IsVisible { get; private set; }

        public OnVisibilityChangeEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}
