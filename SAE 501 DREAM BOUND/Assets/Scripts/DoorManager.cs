using UnityEngine;
using Unity.Netcode;

public class DoorManager : NetworkBehaviour
{
    [SerializeField] private GameObject door; // Référence à la porte
    [SerializeField] private ButtonScript button1; // Référence au premier bouton
    [SerializeField] private ButtonScript button2; // Référence au deuxième bouton

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
        isDoorOpen.OnValueChanged += OnDoorStateChanged; // Synchronisation de l'état de la porte
    }

    public void CheckButtonsState()
    {
        if (isDoorOpen.Value) return; // Si la porte est déjà ouverte, ne rien faire

        if (button1.IsPressed() && button2.IsPressed()) // Si les deux boutons sont pressés
        {
            SetDoorStateServerRpc(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDoorStateServerRpc(bool open)
    {
        isDoorOpen.Value = open; // Met à jour l'état de la porte côté serveur
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
            door.SetActive(false); // Désactive la porte pour "l'ouvrir"
        }
        else
        {
            Debug.LogError("Door reference is missing.");
        }
    }

    private void OnDestroy()
    {
        isDoorOpen.OnValueChanged -= OnDoorStateChanged; // Désabonnement
    }
}
