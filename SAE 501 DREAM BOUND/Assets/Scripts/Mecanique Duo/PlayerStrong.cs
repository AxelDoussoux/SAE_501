using UnityEngine;

public class PlayerStrong : MonoBehaviour
{
    public Transform launchPoint; // Point où le joueur agile sera propulsé
    public float launchForce = 500f; // Force de propulsion
    private GameObject agilePlayerInRange; // Référence au joueur agile proche

    private void OnTriggerEnter(Collider other)
    {
        PlayerAgile agilePlayer = other.GetComponent<PlayerAgile>();
        if (agilePlayer != null)
        {
            agilePlayerInRange = other.gameObject;
            Debug.Log("Joueur agile détecté !");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerAgile agilePlayer = other.GetComponent<PlayerAgile>();
        if (agilePlayer != null && agilePlayerInRange == other.gameObject)
        {
            agilePlayerInRange = null;
            Debug.Log("Joueur agile hors de portée !");
        }
    }

    public void LaunchPlayer()
    {
        if (agilePlayerInRange != null)
        {
            Debug.Log("Lancement du joueur agile !");
            Rigidbody agileRb = agilePlayerInRange.GetComponent<Rigidbody>();
            agileRb.velocity = Vector3.zero;
            agileRb.AddForce(transform.up * launchForce, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("Aucun joueur agile dans la portée !");
        }
    }

}
