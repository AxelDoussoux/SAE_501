using UnityEngine;
using Unity.Netcode;

public class MovementCube : NetworkBehaviour
{
    public Vector3 positionB = Vector3.zero; // Target position, visible in Inspector
    private Vector3 positionA; // Initial position
    public float speed = 5f; // Movement speed
    private NetworkVariable<bool> shouldMove = new NetworkVariable<bool>(false);
    private Rigidbody rb; // Rigidbody component

    // This NetworkVariable will be used to synchronize positionB across all clients
    private NetworkVariable<Vector3> networkPositionB = new NetworkVariable<Vector3>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Retrieve the Rigidbody of the cube
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[CubeMovement] Rigidbody is missing! Please add one to the Cube.");
            return;
        }

        // Save the initial position
        positionA = transform.position;

        // If it's the server, set the position B on the server
        if (IsServer)
        {
            // Ensure position B is in global space, regardless of parent
            networkPositionB.Value = positionB; // Use the global position defined in the inspector
        }

        // Do not use physics (we manually manipulate the position)
        rb.isKinematic = true;

        Debug.Log($"[MovementCube] Initialized - Position A: {positionA}, Position B: {networkPositionB.Value}");
    }

    void Update()
    {
        if (rb == null) return;

        // Use networkPositionB for movement
        Vector3 targetPosition = shouldMove.Value ? networkPositionB.Value : positionA;

        // If the position is still distant, move the cube
        if (Vector3.Distance(rb.position, targetPosition) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetMoveStateServerRpc(bool move, ServerRpcParams serverRpcParams = default)
    {
        // Update the movement state
        shouldMove.Value = move;
        Debug.Log($"[CubeMovement] SetMoveStateServerRpc called with value: {move}");
    }

    // ServerRpc to change position B, and synchronize it with all clients
    [ServerRpc(RequireOwnership = false)]
    public void SetPositionBServerRpc(Vector3 newPositionB, ServerRpcParams serverRpcParams = default)
    {
        // If it's the server, update position B and synchronize with clients
        if (IsServer)
        {
            // Ensure the position is in global space, even if the cube is a child
            Vector3 globalPositionB = newPositionB;

            // Update position B on the server
            networkPositionB.Value = globalPositionB;
            Debug.Log($"[CubeMovement] New positionB set to: {globalPositionB}");
        }
    }

    // Method to bind position B, called by clients if necessary
    public void SetPositionB(Vector3 newPositionB)
    {
        if (IsServer)
        {
            SetPositionBServerRpc(newPositionB);
        }
        else
        {
            // If not the server, request the update of position B
            SetPositionBServerRpc(newPositionB);
        }
    }
}
