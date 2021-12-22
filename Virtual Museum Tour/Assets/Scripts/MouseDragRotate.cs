using UnityEngine;

// http://gyanendushekhar.com/2018/01/11/rotate-gameobject-using-mouse-drag-or-touch-unity-tutorial/

public class MouseDragRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5.0f;

    private void OnMouseDrag()
    {
        float xAxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        float yAxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
        
        transform.Rotate(Vector3.down, -xAxisRotation, Space.World);
        transform.Rotate(Vector3.right, yAxisRotation, Space.World);
    }
}
