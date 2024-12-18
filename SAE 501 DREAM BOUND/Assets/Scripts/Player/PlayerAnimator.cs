using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    private const string VelocityParam = "Velocity";

    [SerializeField] private float velocity = 0f;
    [SerializeField] private float velocityTransitionTime = 65f;

    private void OnEnable()
    {
        // Initialisation des composants s'ils ne sont pas assignés via l'inspecteur
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();  // Créer une nouvelle instance si nécessaire
                Debug.LogWarning("Animator component was not assigned, creating a new one.");
            }
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();  // Créer une nouvelle instance si nécessaire
                Debug.LogWarning("Rigidbody component was not assigned, creating a new one.");
            }
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            Debug.Log("PlayerAnimator: This object is not owned by the local client.");
            return;
        }

        if (rb == null || animator == null)
        {
            Debug.LogWarning("PlayerAnimator: Rigidbody or Animator is not initialized.");
            return;
        }

        // Calculer la vitesse normalisée du joueur
        velocity = Mathf.Clamp01(rb.velocity.magnitude / velocityTransitionTime); // Normaliser la vitesse (par exemple, avec une valeur max de 10)
        animator.SetFloat(VelocityParam, velocity);
    }
}
