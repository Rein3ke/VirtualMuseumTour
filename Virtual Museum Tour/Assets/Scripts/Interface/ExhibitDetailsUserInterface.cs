using System;
using Controller;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Interface
{
    public class ExhibitDetailsUserInterface : MonoBehaviour, IUserInterface
    {
        public static ExhibitDetailsUserInterface Instance { get; private set; }

        [SerializeField] private float scaleFactor = 100f;
        [SerializeField] private GameObject modelHolder;
        [SerializeField] private GameObject imageHolder;
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

            // on right mouse button, hide interface
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

        private GameObject[] GetAttachedImageGameObjects()
        {
            var childCount = imageHolder.transform.childCount;
            var attachedImageGameObjects = new GameObject[childCount];
            for (int index = 0; index < childCount; index++)
            {
                attachedImageGameObjects[index] = imageHolder.transform.GetChild(index).gameObject;
            }

            return attachedImageGameObjects;
        }

        private static void AttachScriptToModel(GameObject model)
        {
            model.AddComponent<MouseDragRotate>();
        }

        private void InterfaceSetup(Exhibit exhibit)
        {
            // reset interface
            Destroy(GetAttachedModel());
            foreach (var imageGameObject in GetAttachedImageGameObjects())
            {
                Destroy(imageGameObject);
            }
            
            // set variables
            title.text = exhibit.Name;
            description.text = exhibit.Description;
            
            // set model
            var attachedGameObject = Instantiate(exhibit.Asset.GetComponentInChildren<Renderer>().gameObject, modelHolder.transform);
            attachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            attachedGameObject.layer = LayerMask.NameToLayer("UI");
            
            // attach Drag And Rotation script to model
            AttachScriptToModel(attachedGameObject);
            
            // set images
            for (var index = 0; index < exhibit.ExhibitData.images.Length; index++)
            {
                var storedTexture = exhibit.ExhibitData.images[index];
                var imageObject = new GameObject($"Image_{index}");
                // set rect transform
                var trans = imageObject.AddComponent<RectTransform>();
                trans.transform.SetParent(imageHolder.transform);
                trans.localScale = Vector3.one;
                trans.anchoredPosition3D = Vector3.zero;
                trans.sizeDelta = new Vector2(100, 100);

                var image = imageObject.AddComponent<Image>();
                image.sprite = Sprite.Create(
                    storedTexture,
                    new Rect(0, 0, storedTexture.width, storedTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
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
