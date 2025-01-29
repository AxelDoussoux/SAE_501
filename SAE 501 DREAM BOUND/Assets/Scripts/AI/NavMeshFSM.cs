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
    private FSMState debugState; 

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

        // Handle AI behavior based on the current state
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
    // Finds the nearest player and assigns them as the target.
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

    // Updates the patrol state behavior. If a player enters the chase radius, switch to chase mode.
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

    // Updates the chase state. The AI follows the player and switches to a faster chase if needed.
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

    // Updates the fast chase state. If the player is close enough, switch to attack mode.
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

    // Updates the attack state. The AI stops moving and attacks the target player.
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

    // Server-side logic for handling the attack on a player.
    [ServerRpc]
    private void ServerAttackPlayerServerRpc(ulong targetClientId)
    {
        Debug.Log($"Attacking player with ClientId: {targetClientId}");
    }

    // Teleports the player back to their spawn point on the server.
    [ServerRpc(RequireOwnership = false)]
    private void TeleportToSpawnPointServerRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
    {
        TeleportPlayerClientRpc(networkObjectId, position, rotation);
    }

    // Teleports the player to the specified position and rotation on the client.
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