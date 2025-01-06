using UnityEngine;
using Unity.Netcode;

public class MovementCube : NetworkBehaviour
{
    public Vector3 positionB = Vector3.zero; // Target position
    private Vector3 positionA; // Initial position
    public float speed = 5f; // Movement speed
    private NetworkVariable<bool> shouldMove = new NetworkVariable<bool>(false);
    private Rigidbody rb; // Rigidbody component

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Cache the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[CubeMovement] Rigidbody is missing! Please add one to the Cube.");
            return;
        }

        positionA = transform.position;

        // Ensure positionB is valid
        if (positionB == Vector3.zero || positionB == positionA)
        {
            Debug.LogWarning($"[CubeMovement] Position B not set, defaulting to Position A: {positionA}");
            positionB = positionA;
        }

        // Ensure the cube starts immobile
        shouldMove.Value = false;
        rb.isKinematic = true; // Ensure physics won't interfere
        Debug.Log($"[CubeMovement] Initialized - Position A: {positionA}, Position B: {positionB}, ShouldMove: {shouldMove.Value}");
    }

    void Update()
    {
        // If Rigidbody is missing, skip the movement logic
        if (rb == null) return;

        // Determine target position
        Vector3 targetPosition = shouldMove.Value ? positionB : positionA;

        // Move the cube towards the target position using Rigidbody
        if (Vector3.Distance(rb.position, targetPosition) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetMoveStateServerRpc(bool move, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"[CubeMovement] SetMoveStateServerRpc called with value: {move}. Sender: {serverRpcParams.Receive.SenderClientId}");
        shouldMove.Value = move;
        Debug.Log($"[CubeMovement] NetworkVariable shouldMove updated to: {shouldMove.Value}");
    }
}
