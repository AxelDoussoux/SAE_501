using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DialogueCursorManager : MonoBehaviour
{
    [SerializeField] private GameObject npc;
    [SerializeField] private GameObject npc2;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private GameObject morpheePatrouillePlayer1;
    [SerializeField] private GameObject morpheePatrouillePlayer2;
    private static bool morpheePatrouilleActivated = false;
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
        
    }

    public void SetActiveMorpheePatrouillePlayer1()
    {
        if (!morpheePatrouilleActivated && morpheePatrouillePlayer1 != null)
            {
                morpheePatrouillePlayer1.SetActive(true);
                morpheePatrouilleActivated = true; // Met à jour le flag pour empêcher les appels futurs
            }
    }

    public void SetActiveMorpheePatrouillePlayer2()
    {
        if (morpheePatrouillePlayer2 != null)
        {
            morpheePatrouillePlayer2.SetActive(true);
            morpheePatrouilleActivated = true; // Met à jour le flag pour empêcher les appels futurs
        }
    }

    public void HideNPC1()
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
    public void HideNPC2()
    {
        if (npc2 != null)
        {
            // Instancie le prefab de particules à la position du NPC
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, npc2.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned in DialogueCursorManager.");
            }
            // Désactive le GameObject du NPC
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
            // Instancie le prefab de particules à la position de Morphee Patrouille
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, morpheePatrouillePlayer1.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned for Morphee Patrouille in DialogueCursorManager.");
            }
            // Désactive le GameObject de Morphee Patrouille
            morpheePatrouillePlayer1.SetActive(false);
            Debug.Log("Morphee Patrouille has disappeared.");
        }
        else
        {
            Debug.LogWarning("Morphee Patrouille GameObject is not assigned in DialogueCursorManager.");
        }
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'écran
        Cursor.visible = false;
    }

    public void HideMorpheePatrouillePlayer2()
    {
        if (morpheePatrouillePlayer1 != null)
        {
            // Instancie le prefab de particules à la position de Morphee Patrouille
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, morpheePatrouillePlayer1.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle Prefab is not assigned for Morphee Patrouille in DialogueCursorManager.");
            }
            // Désactive le GameObject de Morphee Patrouille
            morpheePatrouillePlayer2.SetActive(false);
            Debug.Log("Morphee Patrouille has disappeared.");
        }
        else
        {
            Debug.LogWarning("Morphee Patrouille GameObject is not assigned in DialogueCursorManager.");
        }
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'écran
        Cursor.visible = false;
    }
}