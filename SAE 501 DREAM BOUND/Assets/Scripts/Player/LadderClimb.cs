using Unity.Netcode;
using UnityEngine;

public class LadderClimb : NetworkBehaviour
{
    public float climbSpeed = 3f;
    private NetworkVariable<bool> isClimbing = new NetworkVariable<bool>(false);
    private Rigidbody rb;
    private int ladderContactCount = 0; // Compte le nombre d'échelles en contact

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.GetComponent<LadderComponent>() != null)
        {
            ladderContactCount++;
            SetClimbingStateServerRpc(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.GetComponent<LadderComponent>() != null)
        {
            ladderContactCount--;
            // Ne désactive le mode escalade que si on n'est plus en contact avec aucune échelle
            if (ladderContactCount <= 0)
            {
                ladderContactCount = 0; // Pour éviter les nombres négatifs
                SetClimbingStateServerRpc(false);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetClimbingStateServerRpc(bool state)
    {
        isClimbing.Value = state;
        UpdatePhysicsStateClientRpc(state);
    }

    [ClientRpc]
    private void UpdatePhysicsStateClientRpc(bool state)
    {
        rb.useGravity = !state;
        if (!state)
        {
            rb.velocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (isClimbing.Value)
        {
            float vertical = Input.GetAxis("Vertical");
            if (vertical != 0)
            {
                MoveOnLadderServerRpc(vertical);
            }
            else
            {

            }
        }
    }

    [ServerRpc]
    private void MoveOnLadderServerRpc(float verticalInput)
    {
        Vector3 movement = new Vector3(0, verticalInput * climbSpeed, 0);
        UpdateMovementClientRpc(movement);
    }

    [ClientRpc]
    private void UpdateMovementClientRpc(Vector3 movement)
    {
        rb.velocity = movement;
    }
}