using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using TomAg;

public class NavMeshFSM : NetworkBehaviour
{
    public enum FSMState
    {
        Patrol,
        Chase,
        ChaseFast,
        Attack
    }

    private NetworkVariable<FSMState> curState = new NetworkVariable<FSMState>(FSMState.Patrol);
    private NetworkVariable<ulong> targetPlayerClientId = new NetworkVariable<ulong>();

    [SerializeField]
    private FSMState debugState; // Pour afficher l'état dans l'inspecteur

    public float chaseRadius = 20.0f;
    public float chaseFastRadius = 10.0f;
    public float attackRadius = 2.0f;

    public GameObject[] patrolPoints;
    private int currentPatrolIndex = 0;
    private NavMeshAgent navAgent;
    private NetworkBehaviour lastTargetPlayer;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
            navAgent.destination = patrolPoints[currentPatrolIndex].transform.position;
    }

    void Update()
    {
        if (!IsServer) return;

        // Mettre à jour la variable pour la visualisation dans l'Inspecteur
        debugState = curState.Value;

        if (targetPlayerClientId.Value == 0)
        {
            FindNearestPlayerServerRpc();
        }

        var players = FindObjectsOfType<NetworkPlayer>();
        NetworkBehaviour targetPlayer = null;
        foreach (var player in players)
        {
            if (player.OwnerClientId == targetPlayerClientId.Value)
            {
                targetPlayer = player;
                break;
            }
        }

        if (targetPlayer == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);

        switch (curState.Value)
        {
            case FSMState.Patrol:
                UpdatePatrolState(distanceToPlayer);
                break;
            case FSMState.Chase:
                UpdateChaseState(distanceToPlayer, targetPlayer.transform);
                break;
            case FSMState.ChaseFast:
                UpdateChaseFastState(distanceToPlayer, targetPlayer.transform);
                break;
            case FSMState.Attack:
                UpdateAttackState(distanceToPlayer, targetPlayer);
                break;
        }
    }

    [ServerRpc]
    private void FindNearestPlayerServerRpc()
    {
        float closestDistance = float.MaxValue;
        ulong closestPlayerId = 0;

        var players = FindObjectsOfType<NetworkPlayer>();
        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayerId = player.OwnerClientId;
            }
        }

        targetPlayerClientId.Value = closestPlayerId;
    }

    private void UpdatePatrolState(float distanceToPlayer)
    {
        if (distanceToPlayer <= chaseRadius)
        {
            curState.Value = FSMState.Chase;
            return;
        }
        if (navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.pathPending)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            navAgent.destination = patrolPoints[currentPatrolIndex].transform.position;
        }
    }

    private void UpdateChaseState(float distanceToPlayer, Transform playerTransform)
    {
        navAgent.speed = 3.5f;
        navAgent.destination = playerTransform.position;
        if (distanceToPlayer <= chaseFastRadius)
        {
            curState.Value = FSMState.ChaseFast;
        }
        else if (distanceToPlayer > chaseRadius)
        {
            curState.Value = FSMState.Patrol;
        }
    }

    private void UpdateChaseFastState(float distanceToPlayer, Transform playerTransform)
    {
        navAgent.speed = 6.0f;
        navAgent.destination = playerTransform.position;
        if (distanceToPlayer <= attackRadius)
        {
            curState.Value = FSMState.Attack;
        }
        else if (distanceToPlayer > chaseFastRadius)
        {
            curState.Value = FSMState.Chase;
        }
    }

    private void UpdateAttackState(float distanceToPlayer, NetworkBehaviour targetPlayer)
    {
        navAgent.isStopped = true;
        ServerAttackPlayerServerRpc(targetPlayer.OwnerClientId);


        if (lastTargetPlayer != targetPlayer)
        {
            var playerInfo = targetPlayer.GetComponent<PlayerInfo>();
            if (playerInfo != null)
            {
                TeleportToSpawnPointServerRpc(targetPlayer.NetworkObject.NetworkObjectId, playerInfo.SpawnPoint.position, playerInfo.SpawnPoint.rotation);
            }
            lastTargetPlayer = targetPlayer;
        }

        if (distanceToPlayer > attackRadius)
        {
            navAgent.isStopped = false;
            curState.Value = FSMState.ChaseFast;
            lastTargetPlayer = null;
        }
    }

    [ServerRpc]
    private void ServerAttackPlayerServerRpc(ulong targetClientId)
    {
        Debug.Log($"Attacking player with ClientId: {targetClientId}");
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportToSpawnPointServerRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
    {
        TeleportPlayerClientRpc(networkObjectId, position, rotation);
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
    {
        if (Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            Transform playerTransform = networkObject.transform;
            Debug.Log($"Teleporting player to Position: {position}, Rotation: {rotation}");
            playerTransform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            Debug.LogWarning($"Failed to find NetworkObject with ID {networkObjectId}");
        }
    }
}