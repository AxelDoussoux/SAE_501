using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class DoorManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] doors; // Array of door objects
    [SerializeField] private ButtonScript button1; // Reference to the first button
    [SerializeField] private ButtonScript button2; // Reference to the second button
    [SerializeField] private AudioClip doorOpenSound; // Sound when the door opens
    [SerializeField] private float doorMoveDistance = 15f; // Distance the door moves
    [SerializeField] private float doorMoveTime = 9f; // Duration of the door movement
    [SerializeField] private float soundVolume = 10f; // Volume of the door open sound

    private NetworkVariable<bool> areDoorsOpen = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public static DoorManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of DoorManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        areDoorsOpen.OnValueChanged += OnDoorsStateChanged; // Subscribe to state changes of doors
    }

    // Check if both buttons are pressed and open the doors
    public void CheckButtonsState()
    {
        if (areDoorsOpen.Value) return;

        if (button1.IsPressed() && button2.IsPressed())
        {
            SetDoorsStateServerRpc(); // Open doors on server if both buttons are pressed
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDoorsStateServerRpc()
    {
        if (!areDoorsOpen.Value)
        {
            areDoorsOpen.Value = true; // Open doors on the server
        }
    }

    private void OnDoorsStateChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            OpenDoors(); // Open doors when the state changes to true
        }
    }

    private void OpenDoors()
    {
        if (doors != null && doors.Length > 0)
        {
            // Play sound when doors open
            if (doorOpenSound != null)
            {
                AudioSource.PlayClipAtPoint(doorOpenSound, Vector3.zero, soundVolume);
            }

            // Move doors up
            foreach (GameObject door in doors)
            {
                if (door != null)
                {
                    door.transform.DOMove(
                        door.transform.position + Vector3.up * doorMoveDistance,
                        doorMoveTime
                    );
                }
                else
                {
                    Debug.LogError("One of the door references is missing.");
                }
            }
        }
        else
        {
            Debug.LogError("No doors assigned to the doors array.");
        }
    }

    private void OnDestroy()
    {
        areDoorsOpen.OnValueChanged -= OnDoorsStateChanged; // Unsubscribe from state changes
    }
}
