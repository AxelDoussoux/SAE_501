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

        // NetworkVariables to synchronize states across the network
        private NetworkVariable<float> networkVelocity = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsGrounded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private NetworkVariable<bool> networkIsBreaking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void OnEnable()
        {
            // Initialize required components
            animator ??= GetComponent<Animator>();
            rb ??= GetComponent<Rigidbody>();
            playerMotor ??= GetComponent<PlayerMotor>();
        }

        private void Update()
        {
            // Ensure required components are available
            if (rb == null || animator == null || playerMotor == null)
            {
                Debug.LogWarning("PlayerAnimator: Required components are not initialized.");
                return;
            }

            if (IsOwner)
            {
                // Calculate velocity, grounded, and jumping states for the local player
                float velocity = Mathf.Clamp01(rb.velocity.magnitude / velocityTransitionTime);
                bool isJumping = playerMotor._isJumping;
                bool isGrounded = playerMotor._isGrounded;

                // Sync network variables for velocity, jumping, grounded, and breaking states
                if (networkVelocity.Value != velocity)
                    networkVelocity.Value = velocity;

                if (networkIsGrounded.Value != isGrounded)
                    networkIsGrounded.Value = isGrounded;

                if (networkIsJumping.Value != isJumping)
                    networkIsJumping.Value = isJumping;

                if (networkIsBreaking.Value != isBreaking)
                    networkIsBreaking.Value = isBreaking;

                // Update hammer state on the server
                if (playerMotor.GetComponent<PlayerInfo>().HaveHammer)
                {
                    UpdateHammerStateServerRpc(isBreaking);
                }

                // Set animator parameters for movement, jumping, and breaking states
                animator.SetFloat(VelocityParam, velocity);
                animator.SetBool(IsGroundedParam, isGrounded);
                animator.SetBool(IsJumpingParam, isJumping);
                animator.SetBool(IsBreakingParam, isBreaking);
            }
            else
            {
                // For non-owner clients, use networked values for animator parameters
                animator.SetFloat(VelocityParam, networkVelocity.Value);
                animator.SetBool(IsGroundedParam, networkIsGrounded.Value);
                animator.SetBool(IsJumpingParam, networkIsJumping.Value);
                animator.SetBool(IsBreakingParam, networkIsBreaking.Value);
            }
        }

        // Method to trigger hammer breaking animation
        public void HammerBreak()
        {
            isBreaking = true;
            StartCoroutine(HideHammerCoroutine());
        }

        // Coroutine to hide the hammer after a delay
        private IEnumerator HideHammerCoroutine()
        {
            yield return new WaitForSeconds(1.9f);
            isBreaking = false;
            UpdateHammerStateServerRpc(isBreaking);
        }

        // ServerRPC to update hammer state across the network
        [ServerRpc]
        private void UpdateHammerStateServerRpc(bool isBreaking)
        {
            UpdateHammerStateClientRpc(isBreaking);
        }

        // ClientRPC to update hammer state on the client side
        [ClientRpc]
        private void UpdateHammerStateClientRpc(bool isBreaking)
        {
            if (Hammer1 != null && Hammer2 != null)
            {
                Hammer1.SetActive(!isBreaking);
                Hammer2.SetActive(isBreaking);
            }
        }
    }
}
