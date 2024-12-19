using UnityEngine;
using Unity.Netcode;

namespace TomAg
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimator : NetworkBehaviour
    {
        public static PlayerAnimator Instance;

        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private PlayerMotor playerMotor;

        private const string VelocityParam = "Velocity";
        private const string IsJumpingParam = "IsJumping";
        private const string IsGroundedParam = "IsGrounded";

        [SerializeField] private float velocityTransitionTime = 65f;

        // NetworkVariables pour synchroniser les �tats
        private NetworkVariable<float> networkVelocity = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsGrounded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void OnEnable()
        {
            if (Instance == null) Instance = this;

            // Initialisation des composants s'ils ne sont pas assign�s via l'inspecteur
            animator ??= GetComponent<Animator>();
            rb ??= GetComponent<Rigidbody>();
            playerMotor ??= GetComponent<PlayerMotor>();
        }

        private void Update()
        {
            if (rb == null || animator == null || playerMotor == null)
            {
                Debug.LogWarning("PlayerAnimator: Required components are not initialized.");
                return;
            }

            if (IsOwner)
            {
                // Calculer la vitesse normalis�e du joueur
                float velocity = Mathf.Clamp01(rb.velocity.magnitude / velocityTransitionTime);

                // R�cup�rer les valeurs de `isJumping` et `isGrounded` depuis `PlayerMotor`
                bool isJumping = playerMotor._isJumping;
                bool isGrounded = playerMotor._isGrounded;

                // Mettre � jour les NetworkVariables uniquement si n�cessaire
                if (networkVelocity.Value != velocity)
                    networkVelocity.Value = velocity;

                if (networkIsGrounded.Value != isGrounded)
                    networkIsGrounded.Value = isGrounded;

                if (networkIsJumping.Value != isJumping)
                    networkIsJumping.Value = isJumping;

                // Mettre � jour directement les param�tres pour le propri�taire
                animator.SetFloat(VelocityParam, velocity);
                animator.SetBool(IsGroundedParam, isGrounded);
                animator.SetBool(IsJumpingParam, isJumping);
            }
            else
            {
                // Les autres clients appliquent les �tats re�us via les NetworkVariables
                animator.SetFloat(VelocityParam, networkVelocity.Value);
                animator.SetBool(IsGroundedParam, networkIsGrounded.Value);
                animator.SetBool(IsJumpingParam, networkIsJumping.Value);
            }
        }
    }
}
