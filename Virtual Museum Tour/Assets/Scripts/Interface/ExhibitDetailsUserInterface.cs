using System.Collections.Generic;
using System.Linq;
using Events;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
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

        [Header("Components")] [Space(8)]
        [SerializeField] private GameObject interfaceGameObject;
        [SerializeField] private GameObject modelHolder;
        [SerializeField] private GameObject imageHolder;
        [SerializeField] private Camera interfaceCamera;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI scaleFactorText;
        [SerializeField] private Dropdown audioClipsDropdown;
        
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;

        // Members
        private Exhibit _currentExhibit;
        private GameObject _currentAttachedGameObject;
        private readonly Dictionary<Dropdown.OptionData, AudioClip> _optionsAudioClipsDictionary = new Dictionary<Dropdown.OptionData, AudioClip>();
        private AudioClip _currentAudioClip;
        private bool _isVisible;

        private void Start()
        {
            // Set dropdown value change event
            audioClipsDropdown.onValueChanged.AddListener(delegate { SetCurrentAudioClip(); });
            
            // Set button events
            playButton.onClick.AddListener(delegate { PlaySelectedAudioClip(); });
            pauseButton.onClick.AddListener(delegate { PauseSelectedAudioClip(); });
            
            HideInterface(); // Hide interface
        }

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventExhibitSelect, Initialize);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventExhibitSelect, Initialize);
        }

        private void Initialize(EventParam eventParam)
        {
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

        private void Update()
        {
            if (!_isVisible) return;
            
            // on right mouse button, hide interface
            if (Input.GetMouseButtonDown(1))
            {
                EventManager.TriggerEvent(EventType.EventDetailsInterfaceClose, new EventParam());
                return;
            }

            // update model scale
            var f = Input.mouseScrollDelta.y;
            scaleFactor += f * (scaleFactorChangeSpeed * 4) * Time.deltaTime;
            scaleFactor = Mathf.Clamp(scaleFactor, 1, float.MaxValue);

            // set scale between following bounds
            var meshRenderer = _currentAttachedGameObject.GetComponent<MeshRenderer>();
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
            _currentAttachedGameObject = Instantiate(exhibit.Asset.GetComponentInChildren<Renderer>().gameObject,
                modelHolder.transform);
            _currentAttachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            _currentAttachedGameObject.layer = LayerMask.NameToLayer("UI");
            // set attached game object children
            if (_currentAttachedGameObject.transform.childCount > 0)
            {
                for (int index = 0; index < _currentAttachedGameObject.transform.childCount; index++)
                {
                    var child = _currentAttachedGameObject.transform.GetChild(index).gameObject;
                    if (child.GetComponent<MeshRenderer>() != null) child.layer = LayerMask.NameToLayer("UI");
                }
            }

            // attach Drag And Rotation script to model
            AttachScriptToModel(_currentAttachedGameObject);

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
        }
    }
}