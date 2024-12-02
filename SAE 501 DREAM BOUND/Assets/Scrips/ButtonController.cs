using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public MoveUpAndDown targetPlate; // R�f�rence vers le script de la plaque

    private bool playerNearby = false; // Indique si le joueur est proche

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            targetPlate.ToggleMovement(); // Active/d�sactive le mouvement de la plaque
            Debug.Log("Mouvement de la plaque : " + (targetPlate.isMoving ? "Activ�" : "D�sactiv�"));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true; // Le joueur est proche du bouton
            Debug.Log("Appuyez sur E pour activer/d�sactiver la plaque.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false; // Le joueur s'�loigne
        }
    }
}
