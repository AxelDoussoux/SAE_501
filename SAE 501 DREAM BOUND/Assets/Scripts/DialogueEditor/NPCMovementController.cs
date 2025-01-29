using UnityEngine;
using UnityEngine.AI;

public class NPCMovementController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        // Automatically get the NavMeshAgent attached to this GameObject
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent not found on this GameObject.");
        }
    }

    /// <summary>
    /// Stops the NPC's movement.
    /// </summary>
    public void StopNpc()
    {
        if (navMeshAgent != null)
        {
            // Stop pathfinding and movement
            navMeshAgent.isStopped = true;

            // Reset velocity to immediately stop any residual movement
            navMeshAgent.velocity = Vector3.zero;

            Debug.Log("NPC stopped.");
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not assigned. Unable to stop the NPC.");
        }
    }
}
