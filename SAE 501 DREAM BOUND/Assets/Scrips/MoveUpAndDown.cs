using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    public float moveSpeed = 2f; // Vitesse de d�placement
    public float moveDistance = 3f; // Distance maximale de d�placement
    private Vector3 startPosition;
    public bool isMoving = true; // Indique si la plaque doit bouger

    private void Start()
    {
        startPosition = transform.position; // Enregistrer la position initiale de l'objet
    }

    private void Update()
    {
        if (isMoving) // Bouge uniquement si le mouvement est activ�
        {
            float newY = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = startPosition + new Vector3(0, newY, 0);
        }
    }

    public void ToggleMovement()
    {
        isMoving = !isMoving; // Active/d�sactive le mouvement
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Perdu au contact avec l'objet !");
        }
    }
}
