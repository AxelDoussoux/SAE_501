using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class DoorManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] doors;
    [SerializeField] private ButtonScript button1;
    [SerializeField] private ButtonScript button2;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private float doorMoveDistance = 15f;
    [SerializeField] private float doorMoveTime = 9f;
    [SerializeField] private float soundVolume = 10f; // Valeur entre 0 et 1

    private NetworkVariable<bool> areDoorsOpen = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public static DoorManager Instance { get; private set; }

    private void Awake()
    {
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
        areDoorsOpen.OnValueChanged += OnDoorsStateChanged;
    }

    public void CheckButtonsState()
    {
        if (areDoorsOpen.Value) return;
        if (button1.IsPressed() && button2.IsPressed())
        {
            SetDoorsStateServerRpc(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDoorsStateServerRpc(bool open)
    {
        areDoorsOpen.Value = open;
    }

    private void OnDoorsStateChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        if (doors != null && doors.Length > 0)
        {
            if (doorOpenSound != null)
            {
                AudioSource.PlayClipAtPoint(doorOpenSound, Vector3.zero, soundVolume);
            }

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
        areDoorsOpen.OnValueChanged -= OnDoorsStateChanged;
    }
}