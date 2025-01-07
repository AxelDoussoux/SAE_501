using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PressurePlate : NetworkBehaviour
{
    public List<MovementCube> linkedCubes = new List<MovementCube>(); // Liste des objets à contrôler
    private int objectsOnPlate = 0; // Compteur pour les objets dans la zone

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
    }

    private void OnTriggerEnter(Collider other)
    {
        // Incrémente le compteur pour chaque objet entrant
        objectsOnPlate++;
        Debug.Log($"[PressurePlate] Object entered the plate. Total objects: {objectsOnPlate}");

        // Si c'est le premier objet, activer le mouvement pour tous les cubes
        if (objectsOnPlate == 1)
        {
            ActivateCubes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Décrémente le compteur pour chaque objet sortant
        objectsOnPlate = Mathf.Max(0, objectsOnPlate - 1);
        Debug.Log($"[PressurePlate] Object left the plate. Total objects: {objectsOnPlate}");

        // Si la plaque est vide, désactiver le mouvement pour tous les cubes
        if (objectsOnPlate == 0)
        {
            DeactivateCubes();
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
}
