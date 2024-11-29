using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    public float moveSpeed = 2f; // Vitesse de déplacement
    public float moveDistance = 3f; // Distance maximale de déplacement
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; // Enregistrer la position initiale de l'objet
    }

    private void Update()
    {
        // Déplacement vertical avec une fonction sinus pour un mouvement de montée/descente fluide
        float newY = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = startPosition + new Vector3(0, newY, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifier si l'objet en collision est le joueur
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Perdu au contact avec l'objet !");
            // Vous pouvez ici ajouter la logique pour gérer la perte du jeu, par exemple, arrêter le jeu ou recharger la scène.
        }
    }
}