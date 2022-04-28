using System.Collections.Generic;
using System.Linq;
using Controller;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = Events.EventType;

namespace Interface
{
    /// <summary>
    /// This class controls all user interface controls, as well as user input.
    /// It is used in the ExhibitDetailsUserInterface prefab and configured via the Inspector.
    /// </summary>
    public class ExhibitDetailsUserInterface : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Variable to set the model scale speed. 
        /// </summary>
        [Header("Scale")] [Space(8)]
        [SerializeField] private float scaleFactorChangeSpeed;

        /// <summary>
        /// Variable that holds the model scale.
        /// </summary>
        [SerializeField] private float scaleFactor = 100f;
        /// <summary>
        /// Minimal scale factor value.
        /// </summary>
        [SerializeField] private float minimumScale = 0.7f;
        /// <summary>
        /// Maximal scale factor value.
        /// </summary>
        [SerializeField] private float maximumScale = 4.2f;
        /// <summary>
        /// Variable to define model rotation speed.
        /// </summary>
        [SerializeField] private float modelRotationSpeed = 10f;

        /// <summary>
        /// GameObject that holds the whole detail interface.
        /// </summary>
        [Header("Components")] [Space(8)]
        [SerializeField] private GameObject interfaceGameObject;
        /// <summary>
        /// GameObject that holds the model.
        /// </summary>
        [SerializeField] private GameObject modelHolder;
        /// <summary>
        /// GameObject that contains all images.
        /// </summary>
        [SerializeField] private GameObject imageHolder;
        /// <summary>
        /// Camera that is used to render the interface.
        /// </summary>
        [SerializeField] private Camera interfaceCamera;
        /// <summary>
        /// Model title text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI title;
        /// <summary>
        /// Model description text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI description;
        /// <summary>
        /// Current scale factor text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI scaleFactorText;
        /// <summary>
        /// Dropdown that holds the available audio clips.
        /// </summary>
        [SerializeField] private Dropdown audioClipsDropdown;
        /// <summary>
        /// Area where the mouse can be used to rotate the model.
        /// </summary>
        [SerializeField] private UIElement mouseRotationArea;
        
        // Buttons that are used to start and stop audio playback and to close the interface.
        [Header("Buttons")]
        [SerializeField] private Button playButton, pauseButton, backButton;

        /// <summary>
        /// Button to start an animation.
        /// </summary>
        [Header("Animation")]
        [SerializeField] private Button playAnimationButton;

        #endregion

        #region Members

        /// <summary>
        /// A dictionary that holds all the available audio clips.
        /// The key holds an OptionData object which contains the name of the audio clip. The value holds the actual audio clip.
        /// </summary>
        private readonly Dictionary<Dropdown.OptionData, AudioClip> _optionsAudioClipsDictionary = new Dictionary<Dropdown.OptionData, AudioClip>();

        /// <summary>
        /// List of all animators in the model.
        /// </summary>
        private List<Animator> _animatorList = new List<Animator>();
        /// <summary>
        /// GameObject (model) that is currently attached to the interface.
        /// </summary>
        private GameObject _currentAttachedGameObject;
        /// <summary>
        /// Exhibit (exhibit object) that is currently attached to the interface.
        /// </summary>
        private Exhibit _currentExhibit;
        /// <summary>
        /// Boolean, to check if the animation is playing.
        /// </summary>
        private bool _isAnimationPlaying;
        /// <summary>
        /// Boolean, to check if the interface is currently visible.
        /// </summary>
        private bool _isVisible;

        #endregion

        #region Properties

        /// <summary>
        /// Returns true, if the animation is playing. Also sets the play animation button text.
        /// </summary>
        private bool IsAnimationPlaying
        {
            get => _isAnimationPlaying;
            set
            {
                _isAnimationPlaying = value;
                playAnimationButton.GetComponentInChildren<Text>().text = value ? "Pause animation" : "Play animation";
            }
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Add listeners to all clickable ui elements and hides the interface on start.
        /// </summary>
        private void Start()
        {
            // Set button events
            playButton.onClick.AddListener(PlaySelectedAudioClip);
            pauseButton.onClick.AddListener(PauseSelectedAudioClip);
            playAnimationButton.onClick.AddListener(ToggleAnimation);
            backButton.onClick.AddListener(DisableDetailsInterface);

            HideInterface(); // Hide interface
        }

        /// <summary>
        /// Controls the scaling and rotation of the attached model based on user input.
        /// </summary>
        private void Update()
        {
            if (!_isVisible) return;

            // Update model scale based on user input
            #region Model Scale
            
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
            #endregion
            
            // Set model rotation
            #region Model rotation

            MouseCursorController.SetCursorTexture(mouseRotationArea.MouseOver ? MouseCursorController.DragTexture : null); // Set cursor texture
            
            if (!mouseRotationArea.MouseOver && !mouseRotationArea.IsPressed) return;
            
            var mouseX = Input.GetAxis("Mouse X") * modelRotationSpeed * Time.deltaTime * -1;
            var mouseY = Input.GetAxis("Mouse Y") * modelRotationSpeed * Time.deltaTime;
            var targetEuler = Vector3.zero;
            
            if (mouseRotationArea.IsPressed)
            {
                targetEuler = new Vector3(mouseY, mouseX, 0);
            }
            
            _currentAttachedGameObject.transform.Rotate(targetEuler, Space.World);
            
            #endregion
        }

        /// <summary>
        /// Subscribe to the EventExhibitSelected event and set the Initialize method as the callback.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventExhibitSelect, Initialize);
        }

        /// <summary>
        /// Unsubscribe from the EventExhibitSelected event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventExhibitSelect, Initialize);
        }

        #endregion

        

        #region Animation

        /// <summary>
        /// Toggles between playing and stopping animations.
        /// When the animations are to be played, the list of animators is iterated and each animator within it is activated.
        /// </summary>
        private void ToggleAnimation()
        {
            IsAnimationPlaying = !IsAnimationPlaying;
            
            foreach (var animator in _animatorList)
            {
                animator.enabled = IsAnimationPlaying;
            }
        }

        #endregion

        #region Event Callbacks

        /// <summary>
        /// Callback method to call InterfaceSetup() after the EventExhibitSelect event is invoked. Also sets the current exhibit.
        /// </summary>
        /// <param name="eventParam">The selected exhibit (via eventParam.EventExhibit).</param>
        private void Initialize(EventParam eventParam)
        {
            if (eventParam.EventExhibit == null) return; // Do nothing if the event is called without an exhibit...
            
            _currentExhibit = eventParam.EventExhibit;
            InterfaceSetup(_currentExhibit);
        }

        #endregion

        #region Audio

        /// <summary>
        /// Returns the currently selected AudioClip from the dropdown.
        /// </summary>
        /// <returns>Currently selected AudioClip.</returns>
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

        /// <summary>
        /// Trigger an event to play the currently selected audio clip.
        /// </summary>
        private void PlaySelectedAudioClip()
        {
            var audioClip = GetCurrentlySelectedAudioClip();
            EventManager.TriggerEvent(EventType.EventPlayAudio, new EventParam
            {
                EventAudioClip = audioClip
            });
        }

        /// <summary>
        /// Trigger an event to pause the AudioClip playback.
        /// </summary>
        private void PauseSelectedAudioClip()
        {
            EventManager.TriggerEvent(EventType.EventPauseAudio, new EventParam());
        }

        #endregion

        #region Exhibit

        /// <summary>
        /// Returns the exhibit that is currently set as a child object of the model holder GameObject.
        /// </summary>
        /// <returns>Attached exhibit asset as GameObject.</returns>
        private GameObject GetAttachedModel()
        {
            var child = modelHolder.transform.GetChild(0);

            return child.gameObject != null ? child.gameObject : null;
        }

        /// <summary>
        /// Returns the GameObjects that can be found as child objects of the ImageHolder.
        /// </summary>
        /// <returns>Array of child GameObjects.</returns>
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

        #endregion

        #region Interface

        /// <summary>
        /// Executes an event that announces the closing of the interface.
        /// </summary>
        private void DisableDetailsInterface()
        {
            EventManager.TriggerEvent(EventType.EventDetailsInterfaceClose, new EventParam());
        }
        
        /// <summary>
        /// Sets the user interface and its camera to active. Triggers an event to block the player character's control.
        /// </summary>
        public void ShowInterface()
        {
            interfaceGameObject.SetActive(true);
            interfaceCamera.enabled = true;
            _isVisible = true;
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam
            {
                EventBoolean = true // Lock controls
            });
            MouseCursorController.SetCursorTexture(null); // Reset mouse texture
        }

        /// <summary>
        /// Sets the user interface and its camera to inactive. Triggers an event to unblock the player character's control.
        /// </summary>
        public void HideInterface()
        {
            interfaceGameObject.SetActive(false);
            interfaceCamera.enabled = false;
            _isVisible = false;
            EventManager.TriggerEvent(EventType.EventLockControls, new EventParam
            {
                EventBoolean = false // Unlock controls
            });
        }

        /// <summary>
        /// Resets the user interface and then configures it according to the specified exhibit.
        /// </summary>
        /// <param name="exhibit">The (selected) exhibit. It contains data that is required for the setup.</param>
        private void InterfaceSetup(Exhibit exhibit)
        {
            if (exhibit == null)
            {
                Debug.LogError($"[{nameof(InterfaceSetup)}] Exhibit is null");
                return;
            }
            
            ResetInterface(); // reset interface to its default state

            // set variables
            title.text = exhibit.Name;
            description.text = exhibit.Description;

            // set model
            _currentAttachedGameObject = Instantiate(exhibit.Asset, modelHolder.transform); // instantiate the exhibit asset as a child of the model holder GameObject
            _currentAttachedGameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor); // scale model
            _currentAttachedGameObject.layer = LayerMask.NameToLayer("UIModel"); // set its layer to "UIModel" so will be rendered on top of the UI

            // set layer mask of attached child game objects
            if (_currentAttachedGameObject.transform.childCount > 0)
            {
                foreach (Transform child in _currentAttachedGameObject.transform.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<MeshRenderer>() != null) // set the layer mask of all child GameObjects of the exhibit that have a mesh renderer to the UI model layer.
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("UIModel");
                    }
                }
            }

            // configure the audio clip dropdown
            if (exhibit.AudioClips.Length > 0)
            {
                // create an entry in the dictionary for each AudioClip. With the clip title as key, and the AudioClip as value
                foreach (AudioClip exhibitAudioClip in exhibit.AudioClips)
                {
                    _optionsAudioClipsDictionary.Add(new Dropdown.OptionData($"{exhibitAudioClip.name}"), exhibitAudioClip);
                }
                // convert the dictionary keys to a list (Dropdown.OptionData) and set it as the audio clips dropdown options list
                audioClipsDropdown.options = new List<Dropdown.OptionData>(_optionsAudioClipsDictionary.Keys);
                // set selected audio clip
                _optionsAudioClipsDictionary.Values.FirstOrDefault();
            }
            
            // get animator in main object
            var mainAnimator = _currentAttachedGameObject.GetComponent<Animator>();
            if (mainAnimator != null) _animatorList.Add(mainAnimator);

            foreach (var animator in _currentAttachedGameObject.GetComponentsInChildren<Animator>()) // get all animators inside the child GameObjects and store them in a list
            {
                _animatorList.Add(animator);
            }

            // configure play animation button
            playAnimationButton.interactable = _animatorList.Count > 0;
            playAnimationButton.GetComponentInChildren<Text>().color = playAnimationButton.interactable ? Color.white : Color.gray;

            IsAnimationPlaying = false;
        }

        /// <summary>
        /// Removes the current exhibit and resets the interface to its default state.
        /// </summary>
        private void ResetInterface()
        {
            Destroy(GetAttachedModel()); // destroy attached game object to prevent clipping
            
            scaleFactor = 100f; // reset scale factor to prevent next object being to large/small
            
            foreach (var imageGameObject in GetAttachedImageGameObjects()) // iterate through each attached image and remove their gameObject references
            {
                Destroy(imageGameObject);
            }
            
            _optionsAudioClipsDictionary.Clear(); // clear audio clips dictionary
            
            _animatorList.Clear(); // clear animator list
        }

        #endregion
    }
}