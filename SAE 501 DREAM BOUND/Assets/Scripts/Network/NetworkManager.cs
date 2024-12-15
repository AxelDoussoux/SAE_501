using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using System;
using Unity.Services.Vivox.AudioTaps;

public class NetworkManager : MonoBehaviour
{
    public UnityTransport transport;
    private string joinCode;
    [SerializeField] private MenuUI mainMenuUI;

    [SerializeField] private VivoxChannelAudioTap vivoxChannel;
    [SerializeField] private JoinChannel echoChannel;

    async void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("UnityTransport not found. Please ensure it's attached to the Network Manager.");
            return;
        }

        if (vivoxChannel == null) {
            Debug.LogError("VivoxChannelAudioTap not found.");
            return;
        };
        if (echoChannel == null)
        {
            Debug.LogError("EchoChannel not found.");
            return;
        };

        await Authenticate();

        // Abonner les fonctions pour gérer les déconnexions et l'arrêt du serveur
        Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        Unity.Netcode.NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateMultiplayerRelay(Label codeLabel)
    {
        try
        {
            if (transport == null)
            {
                Debug.LogError("Transport not initialized.");
                return;
            }

            Allocation a = await RelayService.Instance.CreateAllocationAsync(2);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            if (codeLabel != null)
            {
                codeLabel.text = joinCode;
            }
            else
            {
                Debug.LogError("Label for join code is not assigned.");
            }

            transport.SetRelayServerData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
            echoChannel.SetChannelCode(joinCode);

            // Exemple d'utilisation dans un autre script

            Unity.Netcode.NetworkManager.Singleton.StartHost();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating relay: {e.Message}");
        }
    }

    public async void JoinSession(string joinCode)
    {
        try
        {
            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Join code is empty. Please enter a valid code.");
                return;
            }

            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);
            transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
            echoChannel.SetChannelCode(joinCode);
            Unity.Netcode.NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error joining session: {e.Message}");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // Vérifier si le client local est déconnecté
        if (Unity.Netcode.NetworkManager.Singleton.IsClient && clientId == Unity.Netcode.NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Client disconnected from the host. Displaying main menu.");

            if (mainMenuUI != null)
            {
                mainMenuUI.UIVisibility(); // Afficher le menu principal
            }
            else
            {
                Debug.LogError("MainMenuUIDocument is not assigned.");
            }
        }
    }

    private void OnServerStopped(bool obj)
    {
        // Si le serveur est arrêté, afficher le menu principal
        Debug.Log("Server has stopped. Redirecting to the main menu.");

        if (mainMenuUI != null)
        {
            mainMenuUI.UIVisibility(); // Afficher le menu principal
        }
        else
        {
            Debug.LogError("MainMenuUIDocument is not assigned.");
        }
    }

    private void OnDestroy()
    {
        // Désabonner les événements pour éviter les erreurs
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            Unity.Netcode.NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }
}






/*Code du dessus modifié avec ChatGPT car non fonctionnel ci dessous le code initial
 * 
 * 
 * public class NetworkManager : MonoBehaviour
{
    public UnityTransport transport;
    public TMPro.TMP_InputField joinCodeInputField;

    async void Awake()
    {
        await Authenticate();
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Start()
    {
        transport = FindObjectOfType<UnityTransport>();
    }



    public async void CreateMultiplayerRelay()
    {
        Allocation a = await RelayService.Instance.CreateAllocationAsync(2);
        joinCodeInputField.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        transport.SetRelayServerData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        Unity.Netcode.NetworkManager.Singleton.StartHost();
    }

    public void StartHost()
    {
        Unity.Netcode.NetworkManager.Singleton.StartHost();
    }

    public void JoinSession()
    {

    }
}*/