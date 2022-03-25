using System;
using UnityEngine;

namespace Controller
{
    public class MouseCursorController : MonoBehaviour
    {
        private static bool _isVisible;
        [SerializeField] private Texture2D hoverTexture;
        [SerializeField] private Texture2D dragTexture;
        [SerializeField] private Texture2D searchTexture;

        public static Texture2D HoverTexture { get; private set; }
        public static Texture2D DragTexture { get; private set; }
        public static Texture2D SearchTexture { get; private set; }

        public static bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                Cursor.visible = _isVisible;
            }
        }

        private void Awake()
        {
            HoverTexture = hoverTexture;
            DragTexture = dragTexture;
            SearchTexture = searchTexture;
        }

        public static void SetCursorTexture(Texture2D texture)
        {
            var hotspot = texture != null ? new Vector2(texture.width / 2f,texture.height / 2f) : Vector2.zero;
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        }
    }
}