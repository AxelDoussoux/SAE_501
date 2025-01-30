using UnityEngine;
using Unity.Netcode;

public class PressurePlate2 : NetworkBehaviour
{
    public Transform pressurePlate;  
    public Transform plateToMove;   
    public float moveAmount = 1f;    
    public float moveSpeed = 2f;     

    private Vector3 initialPosition;

   
    private void Start()
    {
        initialPosition = plateToMove.localPosition;
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsServer) 
        {
            MovePlateServerRpc(true); 
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && IsServer)  
        {
            MovePlateServerRpc(false);  
        }
    }

 
    [ServerRpc(RequireOwnership = false)]
    void MovePlateServerRpc(bool playerOnPlate, ServerRpcParams rpcParams = default)
    {
       
        if (playerOnPlate)
        {
            StartCoroutine(MovePlate(Vector3.up * moveAmount));
        }
        else
        {
            StartCoroutine(MovePlate(Vector3.zero));
        }
    }

   
    private System.Collections.IEnumerator MovePlate(Vector3 targetPosition)
    {
        Vector3 startPos = plateToMove.localPosition;
        Vector3 endPos = (targetPosition == Vector3.zero) ? initialPosition : startPos + targetPosition;

        float elapsedTime = 0f;

        while (elapsedTime < moveSpeed)
        {
            plateToMove.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        plateToMove.localPosition = endPos;  
    }
}
