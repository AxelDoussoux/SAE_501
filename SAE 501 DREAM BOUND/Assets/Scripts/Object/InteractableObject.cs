using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TomAg
{
    public class InteractableObject : MonoBehaviour
    {

    public TextMeshProUGUI interactText;
    public Image interactBackground;

        public void Interact()
        {

            Debug.Log($"{gameObject.name} a été interacté.");
            ShowInteractText();
            Destroy(gameObject);
        }

         private void ShowInteractText()
        {
            if (interactText != null && interactBackground != null)
            {

                interactText.gameObject.SetActive(true);
                interactBackground.gameObject.SetActive(true);


                Invoke("HideInteractText", 5f);
            }
            else
            {
                Debug.LogWarning("Les références interactText ou interactBackground ne sont pas assignées dans l'inspecteur !");
            }
        }

        private void HideInteractText()
        {

            if (interactText != null && interactBackground != null)
            {
                interactText.gameObject.SetActive(false);
                interactBackground.gameObject.SetActive(false);
            }
        }
    }
}
