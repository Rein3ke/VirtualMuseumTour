using System;
using Controller;
using Events;
using UnityEngine;
using EventType = Events.EventType;

namespace DollHouseView
{
    public enum PoiType
    {
        None = 0,
        Exhibit = 1,
        Exhibition = 2,
        PlayerSpawnPoint = 3
    }

    internal struct GuiLabelSettings
    {
        public readonly float Offset;
        public readonly float Width;
        public readonly float Height;

        public GuiLabelSettings(float offset, float width, float height)
        {
            Offset = offset;
            Width = width;
            Height = height;
        }
    }
    
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SphereCollider))]
    public class PointOfInterest : MonoBehaviour
    {
        public const string PrefabPath = "Prefabs/POI";
        private static readonly Color None = Color.white;
        private static readonly Color Exhibit = Color.yellow;
        private static readonly Color Exhibition = Color.red;
        private static readonly Color PlayerSpawnPoint = Color.magenta;
        private static readonly Color HighlightColor = Color.cyan;

        [SerializeField] private float maxScale = 10f;

        private SphereCollider _collider;
        private bool _isHoverTextVisible;
        private bool _isScaled;
        private bool _isVisible;
        private Camera _mainCamera;
        private GuiLabelSettings guiLabelSettings;
        private PoiType poiType;

        public bool IsClickable { get; set; }

        public string HoverText { get; set; }

        public PoiType PoiType
        {
            get => poiType;
            internal set
            {
                poiType = value;
                ChangeColorBasedOnType();
            }
        }

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            if (!_collider.isTrigger) Debug.LogWarning("POI Collider attribute 'Is Trigger' is set to false!");

            guiLabelSettings = new GuiLabelSettings(-20f, 200f, 200f);
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

        private void OnEnable()
        {
            EventManager.StartListening(EventType.EventDollHouseView, SetMainCamera);
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventType.EventDollHouseView, SetMainCamera);
        }

        private void OnGUI()
        {
            if (!_isHoverTextVisible) return;

            var mousePosition = Input.mousePosition;
            var x = mousePosition.x - guiLabelSettings.Offset;
            var y = Screen.height - mousePosition.y - guiLabelSettings.Offset;

            var rect = new Rect(x, y, guiLabelSettings.Width, guiLabelSettings.Height);

            GUI.Label(rect, HoverText);
        }

        private void OnBecameInvisible()
        {
            _isVisible = false;
        }

        private void OnBecameVisible()
        {
            _isVisible = true;
        }

        private void OnMouseDown()
        {
            if (!IsClickable) return;

            OnClick?.Invoke();
        }

        private void OnMouseEnter()
        {
            _isHoverTextVisible = true;

            if (!IsClickable) return;

            GetComponent<SpriteRenderer>().color = HighlightColor;
            transform.localScale += new Vector3(.25f, .25f, .25f);
            _isScaled = true;
            MouseCursorController.SetCursorTexture(MouseCursorController.HoverTexture);
        }

        private void OnMouseExit()
        {
            _isHoverTextVisible = false;

            if (!IsClickable) return;

            ChangeColorBasedOnType();
            _isScaled = false;
            MouseCursorController.SetCursorTexture(null);
        }

        private void OnValidate()
        {
            ChangeColorBasedOnType();
        }

        private void SetMainCamera(EventParam eventParam)
        {
            _mainCamera = Camera.main;
        }

        public event Action OnClick;

        private void ChangeColorBasedOnType()
        {
            switch (PoiType)
            {
                case PoiType.None:
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

        private void ChangeColor(Color newColor)
        {
            GetComponent<SpriteRenderer>().color = newColor;
        }

        private float CalculateDistance()
        {
            var distance = Vector3.Distance(transform.position, _mainCamera.transform.position);
            return distance;
        }
    }
}