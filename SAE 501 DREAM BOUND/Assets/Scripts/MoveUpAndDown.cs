using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    public float moveSpeed = 2f; // Vitesse de d�placement
    public float moveDistance = 3f; // Distance maximale de d�placement
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; // Enregistrer la position initiale de l'objet
    }

    private void Update()
    {
        // D�placement vertical avec une fonction sinus pour un mouvement de mont�e/descente fluide
        float newY = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = startPosition + new Vector3(0, newY, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // V�rifier si l'objet en collision est le joueur
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Perdu au contact avec l'objet !");
            // Vous pouvez ici ajouter la logique pour g�rer la perte du jeu, par exemple, arr�ter le jeu ou recharger la sc�ne.
        }
    }
}