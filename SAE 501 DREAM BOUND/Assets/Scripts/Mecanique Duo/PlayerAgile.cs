using UnityEngine;

public class PlayerAgile : MonoBehaviour
{
    private PlayerStrong strongPlayerInRange; // R�f�rence au joueur fort proche

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && strongPlayerInRange != null)
        {
            Debug.Log("Touche F press�e, lancement !");
            strongPlayerInRange.LaunchPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerStrong strongPlayer = other.GetComponent<PlayerStrong>();
        if (strongPlayer != null)
        {
            strongPlayerInRange = strongPlayer;
            Debug.Log("Joueur fort d�tect� !");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerStrong strongPlayer = other.GetComponent<PlayerStrong>();
        if (strongPlayer != null && strongPlayerInRange == strongPlayer)
        {
            strongPlayerInRange = null;
            Debug.Log("Joueur fort hors de port�e !");
        }
    }

}
