using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TomAg;

public class DialogueCursorManager : MonoBehaviour
{
    [SerializeField] private GameObject npc;
    [SerializeField] private GameObject npc2;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private GameObject morpheePatrouillePlayer1;
    [SerializeField] private GameObject morpheePatrouillePlayer2;
    private static bool morpheePatrouilleActivated = false;

    public void DisablePlayerControls()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in allPlayers)
        {
            if (player.IsOwner)
            {
                player.SetMovementEnabled(false);
                Debug.Log("Movement controls disabled for local player");
                break;
            }
        }
    }

    public void EnablePlayerControls()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in allPlayers)
        {
            if (player.IsOwner)
            {
                player.SetMovementEnabled(true);
                Debug.Log("Movement controls enabled for local player");
                break;
            }
        }
    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DisablePlayerControls();
    }

    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EnablePlayerControls();
    }

    public void SetActiveMorpheePatrouillePlayer1()
    {
        if (!morpheePatrouilleActivated && morpheePatrouillePlayer1 != null)
        {
            morpheePatrouillePlayer1.SetActive(true);
            morpheePatrouilleActivated = true; // Updates the flag to prevent future calls
        }
    }

    public void SetActiveMorpheePatrouillePlayer2()
    {
        if (morpheePatrouillePlayer2 != null)
        {
            morpheePatrouillePlayer2.SetActive(true);
            morpheePatrouilleActivated = true; // Updates the flag to prevent future calls
        }
    }

    public void HideNPC1()
    {
        if (npc != null)
        {
            // Instantiate the particle prefab at the NPC's position
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, npc.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned in DialogueCursorManager.");
            }
            // Disable the NPC GameObject
            npc.SetActive(false);
        }
        else
        {
            Debug.LogWarning("NPC GameObject is not assigned in DialogueCursorManager.");
        }
    }

    public void HideNPC2()
    {
        if (npc2 != null)
        {
            // Instantiate the particle prefab at the NPC's position
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, npc2.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned in DialogueCursorManager.");
            }
            // Disable the NPC GameObject
            npc2.SetActive(false);
        }
        else
        {
            Debug.LogWarning("NPC GameObject is not assigned in DialogueCursorManager.");
        }
    }

    public void HideMorpheePatrouillePlayer1()
    {
        if (morpheePatrouillePlayer1 != null)
        {
            // Instantiate the particle prefab at the Morphee Patrouille's position
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, morpheePatrouillePlayer1.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned for Morphee Patrouille in DialogueCursorManager.");
            }
            // Disable the Morphee Patrouille GameObject
            morpheePatrouillePlayer1.SetActive(false);
            Debug.Log("Morphee Patrouille has disappeared.");
        }
        else
        {
            Debug.LogWarning("Morphee Patrouille GameObject is not assigned in DialogueCursorManager.");
        }
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false;
    }

    public void HideMorpheePatrouillePlayer2()
    {
        if (morpheePatrouillePlayer1 != null)
        {
            // Instantiate the particle prefab at the Morphee Patrouille's position
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, morpheePatrouillePlayer1.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned for Morphee Patrouille in DialogueCursorManager.");
            }
            // Disable the Morphee Patrouille GameObject
            morpheePatrouillePlayer2.SetActive(false);
            Debug.Log("Morphee Patrouille has disappeared.");
        }
        else
        {
            Debug.LogWarning("Morphee Patrouille GameObject is not assigned in DialogueCursorManager.");
        }
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false;
    }
}
