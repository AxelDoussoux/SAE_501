using Unity.Netcode;
using UnityEngine;

public class LadderClimb : NetworkBehaviour
{
    public float climbSpeed = 3f;  // Speed at which the player climbs
    private NetworkVariable<bool> isClimbing = new NetworkVariable<bool>(false);  // Network variable to track climbing state
    private Rigidbody rb;  // Rigidbody component for controlling physics
    private int ladderContactCount = 0;  // Counts the number of ladders the player is in contact with

    void Start()
    {
        // Initialize the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Triggered when the player collides with a ladder
        if (!IsOwner) return;  // Only process for the owning player

        if (other.GetComponent<LadderComponent>() != null)
        {
            // Increment ladder contact count and start climbing
            ladderContactCount++;
            SetClimbingStateServerRpc(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Triggered when the player exits the ladder
        if (!IsOwner) return;  // Only process for the owning player

        if (other.GetComponent<LadderComponent>() != null)
        {
            // Decrease ladder contact count and stop climbing if no ladders are in contact
            ladderContactCount--;
            if (ladderContactCount <= 0)
            {
                ladderContactCount = 0;  // Avoid negative contact count
                SetClimbingStateServerRpc(false);
            }
        }
    }

    // ServerRPC to set climbing state, called by the owning player
    [ServerRpc(RequireOwnership = false)]
    private void SetClimbingStateServerRpc(bool state)
    {
        isClimbing.Value = state;  // Update the climbing state
        UpdatePhysicsStateClientRpc(state);  // Update the physics state on clients
    }

    // ClientRPC to update the player's physics state based on climbing
    [ClientRpc]
    private void UpdatePhysicsStateClientRpc(bool state)
    {
        rb.useGravity = !state;  // Disable gravity when climbing
        if (!state)
        {
            rb.velocity = Vector3.zero;  // Stop all movement when not climbing
        }
    }

    void FixedUpdate()
    {
        // Called every physics frame, only processes for the owning player
        if (!IsOwner) return;

        if (isClimbing.Value)
        {
            // Get vertical input for climbing
            float vertical = Input.GetAxis("Vertical");
            if (vertical != 0)
            {
                // Move the player on the ladder
                MoveOnLadderServerRpc(vertical);
            }
        }
    }

    // ServerRPC to move the player on the ladder, based on vertical input
    [ServerRpc]
    private void MoveOnLadderServerRpc(float verticalInput)
    {
        // Create movement vector based on input and climb speed
        Vector3 movement = new Vector3(0, verticalInput * climbSpeed, 0);
        UpdateMovementClientRpc(movement);  // Update movement on client
    }

    // ClientRPC to update the player's movement on the ladder
    [ClientRpc]
    private void UpdateMovementClientRpc(Vector3 movement)
    {
        rb.velocity = movement;  // Apply movement to Rigidbody
    }
}
