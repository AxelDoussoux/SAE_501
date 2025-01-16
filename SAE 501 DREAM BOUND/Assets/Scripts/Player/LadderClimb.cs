using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 3f; // Vitesse de mont�e
    private bool isClimbing = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        // V�rifie si l'objet poss�de le script LadderComponent
        if (other.GetComponent<LadderComponent>() != null)
        {
            isClimbing = true;
            rb.useGravity = false; // D�sactive la gravit�
            rb.velocity = Vector3.zero; // Stoppe le mouvement actuel
        }
    }

    void OnTriggerExit(Collider other)
    {
        // V�rifie si l'objet poss�de le script LadderComponent
        if (other.GetComponent<LadderComponent>() != null)
        {
            isClimbing = false;
            rb.useGravity = true; // R�active la gravit�
        }
    }

    void FixedUpdate() // Utilisez FixedUpdate pour la physique
    {
        if (isClimbing)
        {
            float vertical = Input.GetAxis("Vertical"); // Obtenir l'entr�e verticale
            Vector3 climbDirection = new Vector3(0, vertical * climbSpeed, 0);

            // Applique une vitesse verticale directement au Rigidbody
            rb.velocity = climbDirection;
        }
    }
}
