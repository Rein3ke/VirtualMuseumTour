using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface
{
    public class UIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public bool MouseOver { get; private set; }
        public bool IsPressed { get; private set; }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
            IsPressed = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp");
            IsPressed = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseOver = false;
        }
    }
}
