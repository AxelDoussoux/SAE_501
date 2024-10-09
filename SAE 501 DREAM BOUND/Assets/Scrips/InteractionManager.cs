using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TomAg
{
    public class InteractionManager : MonoBehaviour
    {
        public float interactionRange = 3f;
        private Camera _camera;

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

    if (Physics.Raycast(ray, out hit, interactionRange))
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
