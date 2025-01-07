using UnityEngine;

public class FreezeObjectOnContact : MonoBehaviour
{
    public GameObject objectA; // Référence à l'objet A (doit être assigné dans l'inspecteur)
    private Rigidbody rbB;     // Rigidbody de l'objet B
    private Rigidbody rbA;     // Rigidbody de l'objet A
    private bool isInContact = false; // Indique si les objets A et B sont en contact

    public float movementThreshold = 0.1f; // Seuil pour considérer que l'objet A est en mouvement

    private void Start()
    {
        rbB = GetComponent<Rigidbody>();

        if (rbB == null)
        {
            Debug.LogError("L'objet B doit avoir un Rigidbody attaché !");
        }

        if (objectA != null)
        {
            rbA = objectA.GetComponent<Rigidbody>();
            if (rbA == null)
            {
                Debug.LogError("L'objet A doit avoir un Rigidbody attaché !");
            }
        }
        else
        {
            Debug.LogError("L'objet A n'est pas assigné !");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifie si l'objet A est entré en collision avec cet objet (B)
        if (collision.gameObject == objectA)
        {
            isInContact = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Vérifie si l'objet A n'est plus en contact
        if (collision.gameObject == objectA)
        {
            isInContact = false;

            // Libère toutes les contraintes si le contact est rompu
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
            // Vérifie si l'objet A est en mouvement
            if (rbA.velocity.magnitude > movementThreshold)
            {
                // Freeze toutes les positions sauf Y, et toutes les rotations
                rbB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                // Libère les contraintes si A est immobile
                rbB.constraints = RigidbodyConstraints.None;
            }
        }
    }
}
