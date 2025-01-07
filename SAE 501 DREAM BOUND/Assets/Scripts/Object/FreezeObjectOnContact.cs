using UnityEngine;

public class FreezeObjectOnContact : MonoBehaviour
{
    public GameObject objectA; // R�f�rence � l'objet A (doit �tre assign� dans l'inspecteur)
    private Rigidbody rbB;     // Rigidbody de l'objet B
    private Rigidbody rbA;     // Rigidbody de l'objet A
    private bool isInContact = false; // Indique si les objets A et B sont en contact

    public float movementThreshold = 0.1f; // Seuil pour consid�rer que l'objet A est en mouvement

    private void Start()
    {
        rbB = GetComponent<Rigidbody>();

        if (rbB == null)
        {
            Debug.LogError("L'objet B doit avoir un Rigidbody attach� !");
        }

        if (objectA != null)
        {
            rbA = objectA.GetComponent<Rigidbody>();
            if (rbA == null)
            {
                Debug.LogError("L'objet A doit avoir un Rigidbody attach� !");
            }
        }
        else
        {
            Debug.LogError("L'objet A n'est pas assign� !");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // V�rifie si l'objet A est entr� en collision avec cet objet (B)
        if (collision.gameObject == objectA)
        {
            isInContact = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // V�rifie si l'objet A n'est plus en contact
        if (collision.gameObject == objectA)
        {
            isInContact = false;

            // Lib�re toutes les contraintes si le contact est rompu
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
            // V�rifie si l'objet A est en mouvement
            if (rbA.velocity.magnitude > movementThreshold)
            {
                // Freeze toutes les positions sauf Y, et toutes les rotations
                rbB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                // Lib�re les contraintes si A est immobile
                rbB.constraints = RigidbodyConstraints.None;
            }
        }
    }
}
