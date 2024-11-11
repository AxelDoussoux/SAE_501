using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour, IMovable
{
    [SerializeField] private MovementSettings settings;
    private Vector3 initialPosition;
    private Vector3 finalPosition;
    private Coroutine currentMovementCoroutine;

    private void Start()
    {
        InitializePositions();
    }

    private void InitializePositions()
    {
        initialPosition = transform.position;
        finalPosition = initialPosition + settings.moveDirection;
    }

    public void StartMoving(bool moveUp)
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
        Vector3 startPos = transform.position;
        Vector3 endPos = moveUp ? finalPosition : initialPosition;
        currentMovementCoroutine = StartCoroutine(MoveObject(startPos, endPos));
    }

    private IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition)
    {
        float totalDistance = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, endPosition) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * settings.moveSpeed;
            float progressRatio = distanceCovered / totalDistance;
            transform.position = Vector3.Lerp(startPosition, endPosition, progressRatio);
            yield return null;
        }

        transform.position = endPosition;
        currentMovementCoroutine = null;
    }
}