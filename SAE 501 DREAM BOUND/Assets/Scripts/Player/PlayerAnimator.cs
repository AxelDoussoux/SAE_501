using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace TomAg
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimator : NetworkBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private PlayerMotor playerMotor;

        [SerializeField] private GameObject Hammer1;
        [SerializeField] private GameObject Hammer2;

        private const string VelocityParam = "Velocity";
        private const string IsJumpingParam = "IsJumping";
        private const string IsGroundedParam = "IsGrounded";
        private const string IsBreakingParam = "IsBreaking";

        [SerializeField] private float velocityTransitionTime = 65f;

        public bool isBreaking = false;

        // NetworkVariables pour synchroniser les états
        private NetworkVariable<float> networkVelocity = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsGrounded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsBreaking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void OnEnable()
        {
            // Initialisation des composants s'ils ne sont pas assignés via l'inspecteur
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
                // Calculer la vitesse normalisée du joueur
                float velocity = Mathf.Clamp01(rb.velocity.magnitude / velocityTransitionTime);

                // Récupérer les valeurs de `isJumping` et `isGrounded` depuis `PlayerMotor`
                bool isJumping = playerMotor._isJumping;
                bool isGrounded = playerMotor._isGrounded;

                // Mettre à jour les NetworkVariables uniquement si nécessaire
                if (networkVelocity.Value != velocity)
                    networkVelocity.Value = velocity;

                if (networkIsGrounded.Value != isGrounded)
                    networkIsGrounded.Value = isGrounded;

                if (networkIsJumping.Value != isJumping)
                    networkIsJumping.Value = isJumping;

                if (networkIsBreaking.Value != isBreaking)
                    networkIsBreaking.Value = isBreaking;

                if (isBreaking && playerMotor.GetComponent<PlayerInfo>().HaveHammer)
                {
                    Hammer1.gameObject.SetActive(false);
                    Hammer2.gameObject.SetActive(true);
                }

                if (!isBreaking && playerMotor.GetComponent<PlayerInfo>().HaveHammer)
                {
                    Hammer1.gameObject.SetActive(true);
                    Hammer2.gameObject.SetActive(false);
                }

                // Mettre à jour directement les paramètres pour le propriétaire
                animator.SetFloat(VelocityParam, velocity);
                animator.SetBool(IsGroundedParam, isGrounded);
                animator.SetBool(IsJumpingParam, isJumping);
                animator.SetBool(IsBreakingParam, isBreaking);
            }
            else
            {
                // Les autres clients appliquent les états reçus via les NetworkVariables
                animator.SetFloat(VelocityParam, networkVelocity.Value);
                animator.SetBool(IsGroundedParam, networkIsGrounded.Value);
                animator.SetBool(IsJumpingParam, networkIsJumping.Value);
                animator.SetBool(IsBreakingParam, networkIsBreaking.Value);
            }
        }

        public void HammerBreak() 
        { 
            isBreaking = true;
            StartCoroutine(HideHammerCoroutine());
        }

        private IEnumerator HideHammerCoroutine()
        {
            yield return new WaitForSeconds( 1.9f);
            isBreaking = false; 
        }
    }
}
