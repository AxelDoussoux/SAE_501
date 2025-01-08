using UnityEngine;
using Unity.Netcode;

public class DoorManager : NetworkBehaviour
{
    [SerializeField] private GameObject door; // R�f�rence � la porte
    [SerializeField] private ButtonScript button1; // R�f�rence au premier bouton
    [SerializeField] private ButtonScript button2; // R�f�rence au deuxi�me bouton

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
        // Abonne la fonction de synchronisation � la variable r�seau
        isDoorOpen.OnValueChanged += OnDoorStateChanged;
    }

    public void CheckButtonsState()
    {
        // Si les deux boutons sont appuy�s en m�me temps
        if (button1.IsPressed() && button2.IsPressed())
        {
            SetDoorStateServerRpc(true); // Ouvre la porte (appel c�t� serveur)
        }
        else
        {
            SetDoorStateServerRpc(false); // Ferme la porte (appel c�t� serveur)
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDoorStateServerRpc(bool open)
    {
        isDoorOpen.Value = open; // Met � jour la variable r�seau
    }

    private void OnDoorStateChanged(bool oldState, bool newState)
    {
        if (newState)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        door.SetActive(false); // D�sactive la porte pour "l'ouvrir"
    }

    private void CloseDoor()
    {
        door.SetActive(true); // Active la porte pour la "fermer"
    }

    private void OnDestroy()
    {
        // D�sabonne pour �viter des erreurs
        isDoorOpen.OnValueChanged -= OnDoorStateChanged;
    }
}
