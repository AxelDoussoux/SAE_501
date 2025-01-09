using UnityEngine;
using Unity.Netcode;

public class DoorManager : NetworkBehaviour
{
    [SerializeField] private GameObject door; // R�f�rence � la porte
    [SerializeField] private ButtonScript button1; // R�f�rence au premier bouton
    [SerializeField] private ButtonScript button2; // R�f�rence au deuxi�me bouton

    private NetworkVariable<bool> isDoorOpen = new NetworkVariable<bool>(
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
        isDoorOpen.OnValueChanged += OnDoorStateChanged; // Synchronisation de l'�tat de la porte
    }

    public void CheckButtonsState()
    {
        if (isDoorOpen.Value) return; // Si la porte est d�j� ouverte, ne rien faire

        if (button1.IsPressed() && button2.IsPressed()) // Si les deux boutons sont press�s
        {
            SetDoorStateServerRpc(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDoorStateServerRpc(bool open)
    {
        isDoorOpen.Value = open; // Met � jour l'�tat de la porte c�t� serveur
    }

    private void OnDoorStateChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if (door != null)
        {
            door.SetActive(false); // D�sactive la porte pour "l'ouvrir"
        }
        else
        {
            Debug.LogError("Door reference is missing.");
        }
    }

    private void OnDestroy()
    {
        isDoorOpen.OnValueChanged -= OnDoorStateChanged; // D�sabonnement
    }
}
