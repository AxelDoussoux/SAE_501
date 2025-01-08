using UnityEngine;
using Unity.Netcode;

public class DoorManager : NetworkBehaviour
{
    [SerializeField] private GameObject door; // Référence à la porte
    [SerializeField] private ButtonScript button1; // Référence au premier bouton
    [SerializeField] private ButtonScript button2; // Référence au deuxième bouton

    private NetworkVariable<bool> isDoorOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
        // Abonne la fonction de synchronisation à la variable réseau
        isDoorOpen.OnValueChanged += OnDoorStateChanged;
    }

    public void CheckButtonsState()
    {
        // Si la porte est déjà ouverte, ne rien faire
        if (isDoorOpen.Value) return;

        // Si les deux boutons sont appuyés en même temps
        if (button1.IsPressed() && button2.IsPressed())
        {
            SetDoorStateServerRpc(true); // Ouvre la porte (appel côté serveur)
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDoorStateServerRpc(bool open)
    {
        isDoorOpen.Value = open; // Met à jour la variable réseau (ouvre la porte)
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
        door.SetActive(false); // Désactive la porte pour "l'ouvrir"
    }

    private void OnDestroy()
    {
        // Désabonne pour éviter des erreurs
        isDoorOpen.OnValueChanged -= OnDoorStateChanged;
    }
}
