using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; 
    public float stoppingDistance = 1.0f; 
    public bool isRandomPatrol = true; // If enabled, the NPC will patrol randomly

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            // Set the first patrol destination
            SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else
        {
            Debug.LogError("Aucun point de patrouille défini !");
        }
    }

    void Update()
    {
        // Check if the NPC has reached its current destination
        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            // Move to the next patrol point
            NextPatrolPoint();
        }
    }

    /// Sets the NPC's destination to a given position.
    private void SetDestination(Vector3 destination)
    {
        if (agent != null && destination != null)
        {
            agent.SetDestination(destination);
        }
    }

    // Selects the next patrol point, either sequentially or randomly.
    private void NextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        if (isRandomPatrol)
        {
            // Choose a random patrol point
            int randomIndex = Random.Range(0, patrolPoints.Length);
            currentPatrolIndex = randomIndex;
        }
        else
        {
            // Move to the next patrol point in order (loops back at the end)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        SetDestination(patrolPoints[currentPatrolIndex].position);
    }
}
