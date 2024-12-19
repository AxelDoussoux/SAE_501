using UnityEngine;
using Unity.Netcode;
using TomAg;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMotor playerMotor;

    private const string VelocityParam = "Velocity";
    private const string IsJumpingParam = "IsJumping";
    private const string IsGroundedParam = "IsGrounded";

    [SerializeField] private float velocity = 0f;
    [SerializeField] private float velocityTransitionTime = 65f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;

    private void OnEnable()
    {
        // Initialisation des composants s'ils ne sont pas assignés via l'inspecteur
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (playerMotor == null)
        {
            playerMotor = GetComponent<PlayerMotor>();
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

        // Récup valeurs de isJumping et isGrounded de Player Motor
        isJumping = playerMotor._isJumping;
        isGrounded = playerMotor._isGrounded;

        animator.SetFloat(VelocityParam, velocity);
        animator.SetBool(IsGroundedParam, isGrounded);
        animator.SetBool(IsJumpingParam, isJumping);
    }
}
