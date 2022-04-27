using System;
using Controller;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace DollHouseView
{
    /// <summary>
    /// Enumeration to differentiate POI types.
    /// </summary>
    public enum PoiType
    {
        None = 0,
        Exhibit = 1,
        Exhibition = 2,
        PlayerSpawnPoint = 3
    }

    /// <summary>
    /// Structure for setting the size, height and offset of a GUI label.
    /// Used only in the OnGUI method of this PointOfInterest class and is therefore internal.
    /// </summary>
    internal struct GuiLabelSettings
    {
        #region Members

        public readonly float Offset;
        public readonly float Width;
        public readonly float Height;

        #endregion

        #region Constructors

        public GuiLabelSettings(float offset, float width, float height)
        {
            Offset = offset;
            Width = width;
            Height = height;
        }

        #endregion
    }
    
    /// <summary>
    /// Stores information about a point of interest and provides methods to draw a GUI label.
    /// Also provides methods to handle the user interaction.
    ///
    /// This class requires a SpriteRenderer to display the POI texture and a SphereCollider to detect user interaction.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SphereCollider))]
    public class PointOfInterest : MonoBehaviour
    {
        #region Constants
        
        /// <summary>
        /// Path to the prefab in the Resources folder.
        /// </summary>
        public const string PrefabPath = "Prefabs/POI";

        #endregion
        
        #region Static Members
        
        /// <summary>
        /// Display POIs without type in white.
        /// </summary>
        private static readonly Color None = Color.white;
        /// <summary>
        /// Display POIs of type Exhibit in yellow.
        /// </summary>
        private static readonly Color Exhibit = Color.yellow;
        /// <summary>
        /// Display POIs of type Exhibition in red.
        /// </summary>
        private static readonly Color Exhibition = Color.red;
        /// <summary>
        /// Display POIs of type PlayerSpawnPoint in magenta.
        /// </summary>
        private static readonly Color PlayerSpawnPoint = Color.magenta;
        /// <summary>
        /// Display the currently selected POI in cyan.
        /// </summary>
        private static readonly Color HighlightColor = Color.cyan;
        
        #endregion
        
        #region Serialized Fields
        /// <summary>
        /// Maximum scale factor to which the POI should grow when the camera moves away from it.
        /// </summary>
        [SerializeField] private float maxScale = 10f;
        
        #endregion

        #region Members
        
        /// <summary>
        /// SphereCollider component. Used to detect mouse events.
        /// </summary>
        private SphereCollider _collider;
        /// <summary>
        /// Is the hover text currently visible?
        /// </summary>
        private bool _isHoverTextVisible;
        /// <summary>
        /// Is the POI currently scaled up?
        /// </summary>
        private bool _isScaled;
        /// <summary>
        /// Is the POI currently visible?
        /// </summary>
        private bool _isVisible;
        /// <summary>
        /// The camera to rotate the POI to.
        /// </summary>
        private Camera _mainCamera;
        /// <summary>
        /// Reference to the used GUI label settings.
        /// </summary>
        private GuiLabelSettings guiLabelSettings;
        /// <summary>
        /// Type of the POI.
        /// </summary>
        private PoiType poiType;

        #endregion

        #region Properties

        /// <summary>
        /// Boolean to determine if the POI is clickable and can be highlighted.
        /// </summary>
        public bool IsClickable { get; set; }
        /// <summary>
        /// Text to be displayed when hovering over the POI.
        /// </summary>
        public string HoverText { get; set; }

        /// <summary>
        /// Returns the type of the POI.
        /// The type can only be changed by the inspector.
        /// If the type is changed, the color of the POI changes accordingly.
        /// </summary>
        public PoiType PoiType
        {
            get => poiType;
            internal set
            {
                poiType = value;
                ChangeColorBasedOnType();
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Called when the POI is clicked.
        /// Used for inheriting classes to react to the click.
        /// </summary>
        public event Action OnClick;

        #endregion
        
        #region Unity Methods

        /// <summary>
        /// On awake, set the _collider component and the _mainCamera component.
        /// </summary>
        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// If the POI isn't set correctly, display an error message.
        /// All POIs need to have a SphereCollider component that is set to isTrigger to work.
        /// Otherwise, the player would collide with a POI in the scene.
        /// </summary>
        private void Start()
        {
            if (!_collider.isTrigger) Debug.LogWarning("POI Collider attribute 'Is Trigger' is set to false!");

            guiLabelSettings = new GuiLabelSettings(-20f, 200f, 200f); // Initialize the GuiLabelSettings using default values.
        }

        private void Update()
        {
            if (!_isVisible || !_mainCamera) return;

            transform.LookAt(_mainCamera.transform.position);

            if (!_isScaled)
            {
                var calculateDistance = CalculateDistance() * 0.25f;
                var clampedDistance = Mathf.Clamp(calculateDistance, 1f, maxScale);
                var newScale = new Vector3(clampedDistance, clampedDistance, clampedDistance);
                transform.localScale = newScale;
            }
        }

        /// <summary>
        /// When the POI is enabled, subscribes to the EventDollHouseView event that sets the main camera when the DollHouseView is active.
        /// </summary>
        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventDollHouseView, SetMainCamera);
        }

        /// <summary>
        /// Unsubscribes from the EventDollHouseView event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventDollHouseView, SetMainCamera);
        }

        /// <summary>
        /// If _isHoverTextVisible is true display the hover text according to the set GuiLabelSettings.
        /// </summary>
        private void OnGUI()
        {
            if (!_isHoverTextVisible) return; 

            var mousePosition = Input.mousePosition;
            var x = mousePosition.x - guiLabelSettings.Offset;
            var y = Screen.height - mousePosition.y - guiLabelSettings.Offset;

            var rect = new Rect(x, y, guiLabelSettings.Width, guiLabelSettings.Height);
            GUI.Label(rect, HoverText);
        }

        /// <summary>
        /// Sets the _isVisible variable to true if the POI is in the camera's view.
        /// </summary>
        private void OnBecameInvisible()
        {
            _isVisible = false;
        }

        /// <summary>
        /// Sets the _isVisible variable to true if the POI isn't in the camera's view.
        /// </summary>
        private void OnBecameVisible()
        {
            _isVisible = true;
        }

        /// <summary>
        /// Calls the OnClick event if the POI is clicked.
        /// </summary>
        private void OnMouseDown()
        {
            if (!IsClickable) return;

            OnClick?.Invoke();
        }

        /// <summary>
        /// Sets the _isHoverTextVisible variable to true if the POI is hovered.
        /// If the POI is hovered and is clickable the POI is scaled up and highlighted.
        /// </summary>
        private void OnMouseEnter()
        {
            _isHoverTextVisible = true;

            if (!IsClickable) return;

            GetComponent<SpriteRenderer>().color = HighlightColor;
            transform.localScale += new Vector3(.25f, .25f, .25f);
            _isScaled = true;
            MouseCursorController.SetCursorTexture(MouseCursorController.HoverTexture); // Change the mouse cursor to the hover texture.
        }

        /// <summary>
        /// Sets the _isHoverTextVisible variable to false if the POI is no longer hovered.
        /// The POI is no longer highlighted and scaled down.
        /// </summary>
        private void OnMouseExit()
        {
            _isHoverTextVisible = false;

            if (!IsClickable) return;

            ChangeColorBasedOnType(); // Change the color of the POI based on its type.
            _isScaled = false;
            MouseCursorController.SetCursorTexture(null); // Change the mouse cursor to the default cursor.
        }

        /// <summary>
        /// If the POI type was changed in the editor, change the color of the POI.
        /// </summary>
        private void OnValidate()
        {
            ChangeColorBasedOnType();
        }

        #endregion


        #region Color Changing

        /// <summary>
        /// Calls the ChangeColor method with a specified color as parameter.
        /// </summary>
        private void ChangeColorBasedOnType()
        {
            switch (PoiType)
            {
                case PoiType.None: default:
                    ChangeColor(None);
                    break;
                case PoiType.Exhibit:
                    ChangeColor(Exhibit);
                    break;
                case PoiType.Exhibition:
                    ChangeColor(Exhibition);
                    break;
                case PoiType.PlayerSpawnPoint:
                    ChangeColor(PlayerSpawnPoint);
                    break;
            }
        }

        /// <summary>
        /// Gets the SpriteRenderer component of the currently selected POI in the editor and changes its color.
        /// </summary>
        /// <param name="newColor">Color to change to.</param>
        private void ChangeColor(Color newColor)
        {
            GetComponent<SpriteRenderer>().color = newColor;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Sets the main camera if the EventDollHouseView event is triggered.
        /// The DollHouseView doesn't need to be active in the scene.
        /// </summary>
        /// <param name="eventParam">(Obsolete).</param>
        private void SetMainCamera(EventParam eventParam)
        {
            _mainCamera = Camera.main;
        }

        #endregion

        /// <summary>
        /// Returns the calculated distance between the POI and the main camera.
        /// </summary>
        /// <returns>Distance between POI and main camera.</returns>
        private float CalculateDistance()
        {
            var distance = Vector3.Distance(transform.position, _mainCamera.transform.position);
            return distance;
        }
    }
}