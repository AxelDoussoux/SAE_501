using UnityEngine;

public class FreezeObjectOnContact : MonoBehaviour
{
    public GameObject objectA; // Reference to object A (must be assigned in the inspector)
    private Rigidbody rbB;     // Rigidbody of object B
    private Rigidbody rbA;     // Rigidbody of object A
    private bool isInContact = false; // Indicates whether objects A and B are in contact

    public float movementThreshold = 0.1f; // Threshold to consider object A as moving

    private void Start()
    {
        rbB = GetComponent<Rigidbody>();

        if (rbB == null)
        {
            Debug.LogError("Object B must have a Rigidbody attached!");
        }

        if (objectA != null)
        {
            rbA = objectA.GetComponent<Rigidbody>();
            if (rbA == null)
            {
                Debug.LogError("Object A must have a Rigidbody attached!");
            }
        }
        else
        {
            Debug.LogError("Object A is not assigned!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if object A has collided with this object (B)
        if (collision.gameObject == objectA)
        {
            isInContact = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if object A is no longer in contact
        if (collision.gameObject == objectA)
        {
            isInContact = false;

            // Release all constraints when the contact is broken
            if (rbB != null)
            {
                rbB.constraints = RigidbodyConstraints.None;
            }
        }
    }

    private void Update()
    {
        if (isInContact && rbA != null && rbB != null)
        {
            // Check if object A is moving
            if (rbA.velocity.magnitude > movementThreshold)
            {
                // Freeze all positions except Y, and all rotations
                rbB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                // Release the constraints if object A is stationary
                rbB.constraints = RigidbodyConstraints.None;
            }
        }
    }
}
