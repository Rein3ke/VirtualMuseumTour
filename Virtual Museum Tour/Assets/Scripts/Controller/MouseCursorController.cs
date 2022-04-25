using UnityEngine;

namespace Controller
{
    /// <summary>
    /// Sets a texture for the mouse cursor.
    /// All cursor textures are stored in the Inspector.
    /// A hover, drag and search texture are available to choose from.
    /// </summary>
    public class MouseCursorController : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Texture to use for the hover state (e.g. when hovering over a Point of Interest in the DollHouseView).
        /// </summary>
        [SerializeField] private Texture2D hoverTexture;
        /// <summary>
        /// Texture to use for a drag area (e.g. model rotation are in ExhibitDetailsUserInterface).
        /// </summary>
        [SerializeField] private Texture2D dragTexture;
        /// <summary>
        /// Texture to use for a selectable object (e.g. Exhibit in scene).
        /// </summary>
        [SerializeField] private Texture2D searchTexture;

        #endregion

        #region Properties

        /// <summary>
        /// Public accessor for the hover texture.
        /// </summary>
        public static Texture2D HoverTexture { get; private set; }

        /// <summary>
        /// Public accessor for the drag texture.
        /// </summary>
        public static Texture2D DragTexture { get; private set; }

        /// <summary>
        /// Public accessor for the search texture.
        /// </summary>
        public static Texture2D SearchTexture { get; private set; }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Sets the textures defined in the Inspector into the corresponding properties.
        /// </summary>
        private void Awake()
        {
            HoverTexture = hoverTexture;
            DragTexture = dragTexture;
            SearchTexture = searchTexture;
        }

        #endregion

        /// <summary>
        /// Receives a texture and sets it as the cursor texture.
        /// Can be null, in which case the default cursor texture is used.
        /// </summary>
        /// <param name="texture">Desired mouse cursor texture (e.g. MouseCursorController.HoverTexture).</param>
        /// <example>MouseCursorController.SetCursorTexture(MouseCursorController.HoverTexture)</example>
        public static void SetCursorTexture(Texture2D texture)
        {
            // define a hotspot (the point where the texture will be displayed)
            var hotspot = texture != null ? new Vector2(texture.width / 2f,texture.height / 2f) : Vector2.zero;
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        }
    }
}