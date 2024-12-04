using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TomAg
{
    public class InteractionManager : MonoBehaviour
    {
        public float interactionRange = 3f;
        private Camera _camera;

        // Ajoutez une variable LayerMask pour définir les couches à ignorer
        public LayerMask ignoreLayers;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            FindObjectOfType<PlayerController>().onInteract += HandleInteraction;
        }

        private void OnDisable()
        {
            var playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.onInteract -= HandleInteraction;
            }
        }

        private void HandleInteraction()
        {
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            // Modifiez le Raycast pour utiliser le LayerMask
            if (Physics.Raycast(ray, out hit, interactionRange, ~ignoreLayers)) // ~ pour inverser le LayerMask
            {
                Debug.Log($"Raycast hit: {hit.collider.name}");

                if (hit.collider.CompareTag("Interactable"))
                {
                    Debug.Log("Interactable object detected");
                    hit.collider.GetComponent<InteractableObject>()?.Interact();
                }
            }
            else
            {
                Debug.Log("No object hit by Raycast");
            }
        }
    }
}
