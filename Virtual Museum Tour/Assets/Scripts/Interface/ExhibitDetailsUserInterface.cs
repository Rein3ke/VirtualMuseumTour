using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Events;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace Interface
{
    public class ExhibitDetailsUserInterface : MonoBehaviour
    {
        [Header("Scale")] [Space(8)]
        [SerializeField] private float scaleFactorChangeSpeed;

        [SerializeField] private float scaleFactor = 100f;
        [SerializeField] private float minimumScale = 0.7f;
        [SerializeField] private float maximumScale = 4.2f;
        [SerializeField] private float modelRotationSpeed = 10f;

        [Header("Components")] [Space(8)]
        [SerializeField] private GameObject interfaceGameObject;

        [SerializeField] private GameObject modelHolder;
        [SerializeField] private GameObject imageHolder;
        [SerializeField] private Camera interfaceCamera;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI scaleFactorText;
        [SerializeField] private Dropdown audioClipsDropdown;
        [SerializeField] private UIElement mouseRotationArea;

        [Header("Buttons")]
        [SerializeField] private Button playButton, pauseButton, backButton;

        [Header("Animation")]
        [SerializeField] private Button playAnimationButton;

        private readonly Dictionary<Dropdown.OptionData, AudioClip> _optionsAudioClipsDictionary = new Dictionary<Dropdown.OptionData, AudioClip>();

        private List<Animator> _animatorList;
        private GameObject _currentAttachedGameObject;
        private AudioClip _currentAudioClip;
        private Exhibit _currentExhibit;
        private bool _isAnimationPlaying;
        private bool _isVisible;

        private bool IsAnimationPlaying
        {
            get => _isAnimationPlaying;
            set
            {
                _isAnimationPlaying = value;
                playAnimationButton.GetComponentInChildren<Text>().text = value ? "Pause animation" : "Play animation";
            }
        }

        private void Awake()
        {
            _animatorList = new List<Animator>();
        }

        private void Start()
        {
            // Set dropdown value change event
            audioClipsDropdown.onValueChanged.AddListener(delegate { SetCurrentAudioClip(); });
            
            // Set button events
            playButton.onClick.AddListener(PlaySelectedAudioClip);
            pauseButton.onClick.AddListener(PauseSelectedAudioClip);
            playAnimationButton.onClick.AddListener(ToggleAnimation);
            backButton.onClick.AddListener(DisableDetailsInterface);
            
            HideInterface(); // Hide interface
        }

        private void DisableDetailsInterface()
        {
            EventManager.TriggerEvent(EventType.EventDetailsInterfaceClose, new EventParam());
        }
        
        private void Update()
        {
            if (!_isVisible) return;

            // update model scale
            var f = Input.mouseScrollDelta.y;
            scaleFactor += f * (scaleFactorChangeSpeed * 4) * Time.deltaTime;
            scaleFactor = Mathf.Clamp(scaleFactor, 1, float.MaxValue);

            // set scale between following bounds
            var meshRenderer = _currentAttachedGameObject.GetComponentInChildren<MeshRenderer>();
            while (meshRenderer.bounds.extents.x > maximumScale ||
                   meshRenderer.bounds.extents.y > maximumScale ||
                   meshRenderer.bounds.extents.z > maximumScale)
            {
                scaleFactor *= 0.99f;
                _currentAttachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }

            while (meshRenderer.bounds.extents.x < minimumScale ||
                   meshRenderer.bounds.extents.y < minimumScale ||
                   meshRenderer.bounds.extents.z < minimumScale)
            {
                scaleFactor *= 1.01f;
                _currentAttachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            }

            _currentAttachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            // Set scale factor text
            scaleFactorText.text = $"Scale factor: {scaleFactor}";

            #region Model rotation

            // Set model rotation
            MouseCursorController.SetCursorTexture(mouseRotationArea.MouseOver ? MouseCursorController.DragTexture : null);
            
            if (!mouseRotationArea.MouseOver && !mouseRotationArea.IsPressed) return;
            
            var mouseX = Input.GetAxis("Mouse X") * modelRotationSpeed * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * modelRotationSpeed * Time.deltaTime;

            if (mouseRotationArea.IsPressed)
            {
                MouseCursorController.IsVisible = false;
                _currentAttachedGameObject.transform.Rotate(new Vector3(mouseY, mouseX, 0), Space.World);
            }
            else
            {
                MouseCursorController.IsVisible = true;
            }

            #endregion
        }
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventExhibitSelect, Initialize);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventExhibitSelect, Initialize);
        }

        private void ToggleAnimation()
        {
            IsAnimationPlaying = !IsAnimationPlaying;
            
            foreach (var animator in _animatorList)
            {
                animator.enabled = IsAnimationPlaying;
            }
        }

        private void Initialize(EventParam eventParam)
        {
            if (eventParam.EventExhibit == null) return;
            
            _currentExhibit = eventParam.EventExhibit;
            InterfaceSetup(_currentExhibit);
        }

        private void SetCurrentAudioClip()
        {
            var optionData = audioClipsDropdown.options[audioClipsDropdown.value];
            _optionsAudioClipsDictionary.TryGetValue(optionData, out _currentAudioClip);
            Debug.Log($"[{nameof(SetCurrentAudioClip)}] Current audio clip: {_currentAudioClip}");
        }

        private AudioClip GetCurrentlySelectedAudioClip()
        {
            var selectedOptionData = audioClipsDropdown.options[audioClipsDropdown.value]; // Get the currently selected option
            
            if (_optionsAudioClipsDictionary.TryGetValue(selectedOptionData, out var audioClip)) // Try to get the value out of the directory by parsing the selected option data
            {
                Debug.Log($"[{nameof(GetCurrentlySelectedAudioClip)}] Selected audio clip: {audioClip.name}");
                return audioClip;
            }

            return null;
        }

        private void PlaySelectedAudioClip()
        {
            var audioClip = GetCurrentlySelectedAudioClip();
            EventManager.TriggerEvent(EventType.EventPlayAudio, new EventParam
            {
                EventAudioClip = audioClip
            });
        }

        private void PauseSelectedAudioClip()
        {
            EventManager.TriggerEvent(EventType.EventPauseAudio, new EventParam());
        }

        public void ShowInterface()
        {
            interfaceGameObject.SetActive(true);
            interfaceCamera.enabled = true;
            _isVisible = true;
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam
            {
                EventBoolean = true
            });
            MouseCursorController.SetCursorTexture(null); // reset mouse
        }

        public void HideInterface()
        {
            interfaceGameObject.SetActive(false);
            interfaceCamera.enabled = false;
            _isVisible = false;
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam
            {
                EventBoolean = false
            });
        }

        private GameObject GetAttachedModel()
        {
            var child = modelHolder.transform.GetChild(0);

            return child.gameObject != null ? child.gameObject : null;
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

        private static void AttachScriptToModel([NotNull] GameObject model)
        {
            model.AddComponent<MouseDragRotate>();
        }

        private void InterfaceSetup(Exhibit exhibit)
        {
            ResetInterface();

            // set variables
            title.text = exhibit.Name;
            description.text = exhibit.Description;

            // set model
            _currentAttachedGameObject = Instantiate(exhibit.Asset, modelHolder.transform);
            _currentAttachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor); // scale model up (100x times)
            _currentAttachedGameObject.layer = LayerMask.NameToLayer("UI");
            var boxCollider = _currentAttachedGameObject.AddComponent<BoxCollider>();
            boxCollider.size = (new Vector3(scaleFactor, scaleFactor, scaleFactor) / 100f) * 4f;
            
            // set attached game object children
            if (_currentAttachedGameObject.transform.childCount > 0)
            {
                /*for (var index = 0; index < _currentAttachedGameObject.transform.childCount; index++)
                {
                    var child = _currentAttachedGameObject.transform.GetChild(index).gameObject;
                    Debug.Log(child.name);
                    if (child.GetComponent<MeshRenderer>() != null) child.layer = LayerMask.NameToLayer("UI");
                }*/
                foreach (Transform child in _currentAttachedGameObject.transform.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<MeshRenderer>() != null)
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("UI");
                    }
                }
            }

            // attach Drag And Rotation script to model
            // AttachScriptToModel(_currentAttachedGameObject);

            // set audio events
            if (exhibit.AudioClips.Length > 0)
            {
                // save all audio clips into a dictionary including dropdown options which contains an audio clip title each
                foreach (AudioClip exhibitAudioClip in exhibit.AudioClips)
                {
                    _optionsAudioClipsDictionary.Add(new Dropdown.OptionData($"{exhibitAudioClip.name}"), exhibitAudioClip);
                }
                // convert the dictionary keys to a list (Dropdown.OptionData) and set this as the audio clips dropdown options list
                audioClipsDropdown.options = new List<Dropdown.OptionData>(_optionsAudioClipsDictionary.Keys);
                // set selected audio clip
                _currentAudioClip = _optionsAudioClipsDictionary.Values.FirstOrDefault();
            }
            
            // get animator in main object
            var mainAnimator = _currentAttachedGameObject.GetComponent<Animator>();
            if (mainAnimator != null) _animatorList.Add(mainAnimator);
            
            // get animator in child objects
            foreach (var animator in _currentAttachedGameObject.GetComponentsInChildren<Animator>())
            {
                _animatorList.Add(animator);
            }

            playAnimationButton.interactable = _animatorList.Count > 0;

            IsAnimationPlaying = false;

            // set images
            /*for (var index = 0; index < exhibit.ExhibitData.images.Length; index++)
            {
                var storedTexture = exhibit.ExhibitData.images[index];
                var imageObject = new GameObject($"Image_{index}");
                // set rect transform
                var trans = imageObject.AddComponent<RectTransform>();
                trans.transform.SetParent(imageHolder.transform);
                trans.localScale = Vector3.one;
                trans.anchoredPosition3D = Vector3.zero;
                trans.sizeDelta = new Vector2(storedTexture.width, storedTexture.height);

                var image = imageObject.AddComponent<Image>();
                image.sprite = Sprite.Create(
                    storedTexture,
                    new Rect(0, 0, storedTexture.width, storedTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }*/
        }

        private void ResetInterface()
        {
            // destroy attached game object to prevent clipping
            Destroy(GetAttachedModel());
            // reset scale factor to prevent next object being to large
            scaleFactor = 100f;
            // iterate through each attached image and remove their gameObject references
            foreach (var imageGameObject in GetAttachedImageGameObjects())
            {
                Destroy(imageGameObject);
            }
            // clear dictionary
            _optionsAudioClipsDictionary.Clear();
            // clear animator list
            _animatorList.Clear();
        }
    }
}