using UnityEngine;

namespace Interface
{
    public class ExhibitDetailsUserInterface : MonoBehaviour
    {
        [SerializeField] private GameObject modelHolder;
        [SerializeField] private Camera interfaceCamera;

        private void Start() {
            // hide interface at start
            DeactivateInterface();
        }

        public void ActivateInterface()
        {
            interfaceCamera.enabled = true;
        }

        public void DeactivateInterface()
        {
            interfaceCamera.enabled = false;
        }
        
        private GameObject GetAttachedModelAsGameObject()
        {
            var attachedModel = modelHolder.transform.GetChild(0).gameObject;

            return attachedModel != null ? attachedModel : null;
        }

        private void AttachScriptToModel(GameObject model)
        {
            model.AddComponent<MouseDragRotate>();
        }
    }
}
