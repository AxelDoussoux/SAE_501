using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PressurePlateController : NetworkBehaviour
{
    [SerializeField] private GameObject movingBlock;
    [SerializeField] private Vector3 moveDistance = new Vector3(0, 2, 0);
    [SerializeField] private float moveSpeed = 2f;

    private NetworkTransform blockNetworkTransform;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private NetworkVariable<bool> isActivated = new NetworkVariable<bool>();

    void Start()
    {
        blockNetworkTransform = movingBlock.GetComponent<NetworkTransform>();
        if (!blockNetworkTransform)
        {
            blockNetworkTransform = movingBlock.AddComponent<NetworkTransform>();
        }

        initialPosition = movingBlock.transform.position;
        targetPosition = initialPosition + moveDistance;
    }

    void Update()
    {
        if (!IsServer) return;

        Vector3 targetPos = isActivated.Value ? targetPosition : initialPosition;
        movingBlock.transform.position = Vector3.MoveTowards(
            movingBlock.transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Player")) isActivated.Value = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Player")) isActivated.Value = false;
    }
}