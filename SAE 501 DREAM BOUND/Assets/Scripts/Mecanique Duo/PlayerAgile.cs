using UnityEngine;

public class PlayerAgile : MonoBehaviour
{
    private PlayerStrong strongPlayerInRange; // Référence au joueur fort proche

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && strongPlayerInRange != null)
        {
            Debug.Log("Touche F pressée, lancement !");
            strongPlayerInRange.LaunchPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerStrong strongPlayer = other.GetComponent<PlayerStrong>();
        if (strongPlayer != null)
        {
            strongPlayerInRange = strongPlayer;
            Debug.Log("Joueur fort détecté !");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerStrong strongPlayer = other.GetComponent<PlayerStrong>();
        if (strongPlayer != null && strongPlayerInRange == strongPlayer)
        {
            strongPlayerInRange = null;
            Debug.Log("Joueur fort hors de portée !");
        }
    }

}
