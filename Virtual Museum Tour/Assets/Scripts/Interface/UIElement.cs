using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface
{
    /// <summary>
    /// Inherits multiple IPointer-related interfaces to handle mouse input.
    /// Can be added as a component to an UI GameObject to detect and handle mouse input.
    /// </summary>
    public class UIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Determines if the mouse is currently hovering over this UI element.
        /// </summary>
        public bool MouseOver { get; private set; }
        /// <summary>
        /// Determines if the mouse is currently pressed down on this UI element.
        /// </summary>
        public bool IsPressed { get; private set; }
        
        /// <summary>
        /// Gets triggered when the mouse is clicked on this UI element.
        /// </summary>
        /// <param name="eventData">(Unused).</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
        }
        
        /// <summary>
        /// Gets triggered when the mouse is released on this UI element.
        /// </summary>
        /// <param name="eventData">(Unused).</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
        }

        /// <summary>
        /// Gets triggered when the mouse enters this UI element.
        /// </summary>
        /// <param name="eventData">(Unused).</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseOver = true;
        }

        /// <summary>
        /// Gets triggered when the mouse exits this UI element.
        /// </summary>
        /// <param name="eventData">(Unused).</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            MouseOver = false;
        }
    }
}
