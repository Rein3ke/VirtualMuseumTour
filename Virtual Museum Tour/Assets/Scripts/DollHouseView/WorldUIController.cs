using UnityEngine;

namespace DollHouseView
{
    public class WorldUIController : MonoBehaviour
    {
        [SerializeField] private Transform target;
    
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            _rect.LookAt(target);
        }

        public void ButtonClickEvent()
        {
            Debug.Log("Clicked!");
        }
    }
}
