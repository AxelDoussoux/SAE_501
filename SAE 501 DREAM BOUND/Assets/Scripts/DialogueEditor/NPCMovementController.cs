using UnityEngine;
using UnityEngine.AI;

public class NPCMovementController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        // R�cup�rer automatiquement le NavMeshAgent attach� � ce GameObject
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent introuvable sur ce GameObject.");
        }
    }

    /// <summary>
    /// Arr�te le mouvement du NPC.
    /// </summary>
    public void StopNpc()
    {
        if (navMeshAgent != null)
        {
            // Arr�te le calcul du chemin et le mouvement
            navMeshAgent.isStopped = true;

            // R�initialise la v�locit� pour stopper imm�diatement tout mouvement r�siduel
            navMeshAgent.velocity = Vector3.zero;

            Debug.Log("NPC arr�t�.");
        }
        else
        {
            Debug.LogWarning("NavMeshAgent n'est pas assign�. Impossible d'arr�ter le NPC.");
        }
    }
}
