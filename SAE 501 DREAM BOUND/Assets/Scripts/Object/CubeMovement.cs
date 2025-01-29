using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CubeMovement : NetworkBehaviour, IMovable
{
    [SerializeField]
    private MovementSettings settings; // Movement settings for the cube
    private Vector3 initialPosition; // Initial position of the cube
    private Vector3 finalPosition; // Final position of the cube
    private Coroutine currentMovementCoroutine; // To store the current movement coroutine
    private Rigidbody rbPlatform; // Rigidbody of the platform

    private void Start()
    {
        InitializePositions(); // Initialize positions based on movement settings

        Rigidbody rbPlaform = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    private void InitializePositions()
    {
        initialPosition = transform.position; // Set the initial position
        finalPosition = initialPosition + settings.moveDirection; // Calculate the final position
    }

    public void StartMoving(bool moveUp)
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine); // Stop any existing movement coroutine
        }

        Vector3 startPos = transform.position; // Get the current position
        Vector3 endPos = moveUp ? finalPosition : initialPosition; // Determine the target position based on the move direction
        currentMovementCoroutine = StartCoroutine(MoveObject(startPos, endPos)); // Start a new movement coroutine
    }

    private IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition)
    {
        float totalDistance = Vector3.Distance(startPosition, endPosition); // Calculate the total distance to travel
        float startTime = Time.time; // Record the start time

        while (Vector3.Distance(transform.position, endPosition) > 0.01f) // Continue moving until the target is reached
        {
            float distanceCovered = (Time.time - startTime) * settings.moveSpeed; // Calculate how far the object has moved
            float progressRatio = distanceCovered / totalDistance; // Calculate progress ratio
            transform.position = Vector3.Lerp(startPosition, endPosition, progressRatio); // Move the object smoothly
            yield return null; // Wait until the next frame
        }

        transform.position = endPosition; // Set the position to the exact final position

        currentMovementCoroutine = null; // Reset the coroutine reference
    }
}
