using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    public Transform[] patrolPoints; // Points de patrouille à définir dans l'éditeur
    public float stoppingDistance = 1.0f; // Distance minimale pour atteindre un point
    public bool isRandomPatrol = false; // Si activé, le NPC patrouillera de manière aléatoire

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            // Définir le premier point de destination
            SetDestination(patrolPoints[currentPatrolIndex].position);
        }
        else
        {
            Debug.LogError("Aucun point de patrouille défini !");
        }
    }

    void Update()
    {
        // Vérifie si le NPC a atteint sa destination
        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            // Passer au prochain point
            NextPatrolPoint();
        }
    }

    private void SetDestination(Vector3 destination)
    {
        if (agent != null && destination != null)
        {
            agent.SetDestination(destination);
        }
    }

    private void NextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        if (isRandomPatrol)
        {
            // Choisir un point de manière aléatoire
            int randomIndex = Random.Range(0, patrolPoints.Length);
            currentPatrolIndex = randomIndex;
        }
        else
        {
            // Passer au point suivant (boucle si on est au dernier point)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        SetDestination(patrolPoints[currentPatrolIndex].position);
    }
}
