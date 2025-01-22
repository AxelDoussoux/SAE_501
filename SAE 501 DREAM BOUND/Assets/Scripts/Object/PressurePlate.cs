using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using DG.Tweening;

public class PressurePlate : NetworkBehaviour
{
    public List<MovementCube> linkedCubes = new List<MovementCube>();
    private int objectsOnPlate = 0;

    // Param�tres pour l'animation
    [Header("Animation Settings")]
    [SerializeField] private float moveDownDistance = 0.1f; // Distance de descente en unit�s
    [SerializeField] private float animationDuration = 0.3f; // Dur�e de l'animation
    private Vector3 originalPosition;

    // Param�tres pour le son
    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressureSound;

    // NetworkVariable pour synchroniser l'�tat d'animation
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false);

    private void Start()
    {
        originalPosition = transform.position;

        // V�rification des composants n�cessaires
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

        // S'abonner au changement d'�tat
        isPressed.OnValueChanged += OnPressedStateChanged;
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsOnPlate++;
        Debug.Log($"[PressurePlate] Object entered the plate. Total objects: {objectsOnPlate}");

        if (objectsOnPlate == 1 && IsServer)
        {
            isPressed.Value = true;

            // R�cup�rer le NetworkObject du joueur
            var playerObject = other.GetComponent<NetworkObject>();
            if (playerObject != null)
            {
                // Cr�er les param�tres RPC pour cibler sp�cifiquement le client
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerObject.OwnerClientId }
                    }
                };

                // Appeler le PlaySoundClientRpc avec les param�tres
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

    // G�rer les changements d'�tat sur tous les clients
    private void OnPressedStateChanged(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            // Animation de descente
            transform.DOMove(originalPosition + Vector3.down * moveDownDistance, animationDuration)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            // Animation de remont�e
            transform.DOMove(originalPosition, animationDuration)
                .SetEase(Ease.OutQuad);
        }
    }

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
        // Nettoyer les tweens � la destruction
        DOTween.Kill(transform);
    }
}
