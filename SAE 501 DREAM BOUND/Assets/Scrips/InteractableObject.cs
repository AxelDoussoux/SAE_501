using UnityEngine;

namespace TomAg
{
    public class InteractableObject : MonoBehaviour
    {
        public void Interact()
        {

            Debug.Log($"{gameObject.name} a �t� interact�.");
            Destroy(gameObject);
        }
    }
}
