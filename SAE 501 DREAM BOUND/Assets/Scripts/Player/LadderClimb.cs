using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 3f; // Vitesse de montée
    private bool isClimbing = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet possède le script LadderComponent
        if (other.GetComponent<LadderComponent>() != null)
        {
            isClimbing = true;
            rb.useGravity = false; // Désactive la gravité
            rb.velocity = Vector3.zero; // Stoppe le mouvement actuel
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Vérifie si l'objet possède le script LadderComponent
        if (other.GetComponent<LadderComponent>() != null)
        {
            isClimbing = false;
            rb.useGravity = true; // Réactive la gravité
        }
    }

    void FixedUpdate() // Utilisez FixedUpdate pour la physique
    {
        if (isClimbing)
        {
            float vertical = Input.GetAxis("Vertical"); // Obtenir l'entrée verticale
            Vector3 climbDirection = new Vector3(0, vertical * climbSpeed, 0);

            // Applique une vitesse verticale directement au Rigidbody
            rb.velocity = climbDirection;
        }
    }
}
