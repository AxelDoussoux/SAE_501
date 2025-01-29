using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpAndDown : MonoBehaviour
{
    public float moveSpeed = 2f; // Movement speed
    public float moveDistance = 3f; // Maximum movement distance
    private Vector3 startPosition; // Store the initial position
    public bool isMoving = true; // Indicates whether the plate should move

    private void Start()
    {
        startPosition = transform.position; // Record the initial position of the object
    }

    private void Update()
    {
        if (isMoving) // Only move if movement is enabled
        {
            // Calculate new Y position using sine wave to move up and down
            float newY = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = startPosition + new Vector3(0, newY, 0);
        }
    }

    // Toggle the movement state (on/off)
    public void ToggleMovement()
    {
        isMoving = !isMoving; // Toggle movement state
    }

    // Detect collisions with the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Lost on contact with the object!");
        }
    }
}
