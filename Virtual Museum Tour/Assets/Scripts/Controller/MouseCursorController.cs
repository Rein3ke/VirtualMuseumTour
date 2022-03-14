using System;
using UnityEngine;

namespace Controller
{
    public class MouseCursorController : MonoBehaviour
    {
        [SerializeField] private Texture2D hoverTexture;
        [SerializeField] private Texture2D dragTexture;
        [SerializeField] private Texture2D searchTexture;

        public static Texture2D HoverTexture { get; private set; }
        public static Texture2D DragTexture { get; private set; }
        public static Texture2D SearchTexture { get; private set; }

        public static void SetCursorTexture(Texture2D texture)
        {
            var hotspot = texture != null ? new Vector2(texture.width / 2f,texture.height / 2f) : Vector2.zero;
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        }

        private void Awake()
        {
            HoverTexture = hoverTexture;
            DragTexture = dragTexture;
            SearchTexture = searchTexture;
        }
    }
}