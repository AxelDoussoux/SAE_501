using UnityEngine;
using UnityEngine.AI;

public class NPCMovementController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        // Récupérer automatiquement le NavMeshAgent attaché à ce GameObject
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent introuvable sur ce GameObject.");
        }
    }

    /// <summary>
    /// Arrête le mouvement du NPC.
    /// </summary>
    public void StopNpc()
    {
        if (navMeshAgent != null)
        {
            // Arrête le calcul du chemin et le mouvement
            navMeshAgent.isStopped = true;

            // Réinitialise la vélocité pour stopper immédiatement tout mouvement résiduel
            navMeshAgent.velocity = Vector3.zero;

            Debug.Log("NPC arrêté.");
        }
        else
        {
            Debug.LogWarning("NavMeshAgent n'est pas assigné. Impossible d'arrêter le NPC.");
        }
    }
}
