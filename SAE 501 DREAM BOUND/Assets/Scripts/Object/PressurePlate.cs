using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using DG.Tweening;

public class PressurePlate : NetworkBehaviour
{
    public List<MovementCube> linkedCubes = new List<MovementCube>();
    private int objectsOnPlate = 0;

    // Animation settings
    [Header("Animation Settings")]
    [SerializeField] private float moveDownDistance = 0.1f; // Distance to move down in units
    [SerializeField] private float animationDuration = 0.3f; // Duration of the animation
    private Vector3 originalPosition;

    // Sound settings
    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressureSound;

    // NetworkVariable to synchronize the animation state
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false);

    private void Start()
    {
        originalPosition = transform.position;

        // Check if necessary components are present
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (linkedCubes == null || linkedCubes.Count == 0)
        {
            Debug.LogWarning("[PressurePlate] No linked cubes assigned to the pressure plate.");
        }
        else
        {
            Debug.Log($"[PressurePlate] Initialized with {linkedCubes.Count} linked objects.");
        }

        // Subscribe to state changes
        isPressed.OnValueChanged += OnPressedStateChanged;
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsOnPlate++;
        Debug.Log($"[PressurePlate] Object entered the plate. Total objects: {objectsOnPlate}");

        if (objectsOnPlate == 1 && IsServer)
        {
            isPressed.Value = true;

            // Get the NetworkObject of the player
            var playerObject = other.GetComponent<NetworkObject>();
            if (playerObject != null)
            {
                // Create RPC parameters to target the specific client
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerObject.OwnerClientId }
                    }
                };

                // Call PlaySoundClientRpc with parameters
                PlaySoundClientRpc(clientRpcParams);
            }

            ActivateCubes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        objectsOnPlate = Mathf.Max(0, objectsOnPlate - 1);
        Debug.Log($"[PressurePlate] Object left the plate. Total objects: {objectsOnPlate}");

        if (objectsOnPlate == 0 && IsServer)
        {
            isPressed.Value = false;
            DeactivateCubes();
        }
    }

    // Handle state changes across all clients
    private void OnPressedStateChanged(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            // Move the plate down with animation
            transform.DOMove(originalPosition + Vector3.down * moveDownDistance, animationDuration)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            // Move the plate back up with animation
            transform.DOMove(originalPosition, animationDuration)
                .SetEase(Ease.OutQuad);
        }
    }

    // Activate linked cubes' movement
    private void ActivateCubes()
    {
        foreach (var cube in linkedCubes)
        {
            if (cube != null)
            {
                Debug.Log($"[PressurePlate] Activating movement for: {cube.gameObject.name}");
                cube.SetMoveStateServerRpc(true);
            }
        }
    }

    // Deactivate linked cubes' movement
    private void DeactivateCubes()
    {
        foreach (var cube in linkedCubes)
        {
            if (cube != null)
            {
                Debug.Log($"[PressurePlate] Deactivating movement for: {cube.gameObject.name}");
                cube.SetMoveStateServerRpc(false);
            }
        }
    }

    // ClientRpc to play sound
    [ClientRpc]
    private void PlaySoundClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (pressureSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pressureSound);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        // Clean up tweens on destroy
        DOTween.Kill(transform);
    }
}
