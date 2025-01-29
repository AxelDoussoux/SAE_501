using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public MoveUpAndDown targetPlate; // Reference to the plate's movement script

    private bool playerNearby = false; // Indicates if the player is nearby

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            targetPlate.ToggleMovement(); // Toggle the movement of the plate
            Debug.Log("Plate movement: " + (targetPlate.isMoving ? "Enabled" : "Disabled"));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true; // Player is close to the button
            Debug.Log("Press E to toggle the plate's movement.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false; // Player has moved away
        }
    }
}
