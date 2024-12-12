using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCursorManager : MonoBehaviour
{
    [SerializeField] private GameObject npc;
    [SerializeField] private GameObject particlePrefab;

    // Appelé lorsque le dialogue commence
    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Libère le curseur
        Cursor.visible = true; // Rendre le curseur visible
    }

    // Appelé lorsque le dialogue se termine
    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'écran
        Cursor.visible = false; // Cache le curseur
        HideNPC();
    }

    private void HideNPC()
    {
        if (npc != null)
        {
            // Instancie le prefab de particules à la position du NPC
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, npc.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned in DialogueCursorManager.");
            }

            // Désactive le GameObject du NPC
            npc.SetActive(false);
        }
        else
        {
            Debug.LogWarning("NPC GameObject is not assigned in DialogueCursorManager.");
        }
    }
}
