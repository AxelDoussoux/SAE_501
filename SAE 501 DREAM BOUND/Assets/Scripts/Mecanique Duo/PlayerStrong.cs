using UnityEngine;

public class PlayerStrong : MonoBehaviour
{
    public Transform launchPoint; // Point où le joueur agile sera propulsé
    public float launchForce = 500f; // Force de propulsion
    private GameObject agilePlayerInRange; // Référence au joueur agile proche

    // Called when another collider enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider has a PlayerAgile component
        PlayerAgile agilePlayer = other.GetComponent<PlayerAgile>();
        if (agilePlayer != null)
        {
            agilePlayerInRange = other.gameObject; // Store the reference to the agile player
            Debug.Log("Joueur agile détecté !"); // Log the detection of the agile player
        }
    }

    // Called when another collider exits the trigger area
    private void OnTriggerExit(Collider other)
    {
        // Check if the other collider has a PlayerAgile component and is the same player
        PlayerAgile agilePlayer = other.GetComponent<PlayerAgile>();
        if (agilePlayer != null && agilePlayerInRange == other.gameObject)
        {
            agilePlayerInRange = null; // Clear the reference to the agile player
            Debug.Log("Joueur agile hors de portée !"); // Log when the agile player leaves the range
        }
    }

    // Launches the agile player if one is within range
    public void LaunchPlayer()
    {
        if (agilePlayerInRange != null)
        {
            Debug.Log("Lancement du joueur agile !"); // Log the launch attempt
            Rigidbody agileRb = agilePlayerInRange.GetComponent<Rigidbody>(); // Get the rigidbody of the agile player
            agileRb.velocity = Vector3.zero; // Reset the velocity to prevent residual forces
            agileRb.AddForce(transform.up * launchForce, ForceMode.Impulse); // Apply a force to launch the agile player
        }
        else
        {
            Debug.Log("Aucun joueur agile dans la portée !"); // Log if no agile player is in range
        }
    }
}
