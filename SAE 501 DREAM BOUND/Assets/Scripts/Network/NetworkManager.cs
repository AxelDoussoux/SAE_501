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

public class NetworkManager : NetworkBehaviour
{
    private NetworkManager Instance;
    public UnityTransport transport;
    private string joinCode;
    [SerializeField] private MenuUI mainMenuUI;

    [SerializeField] private JoinChannel echoChannel;

    private async void Awake()
    {
        if (Instance == null) Instance = this;

        transport = FindObjectOfType<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("UnityTransport not found. Please ensure it's attached to the Network Manager.");
            return;
        }

        if (echoChannel == null)
        {
            Debug.Log("EchoChannel not found.");
        };

        await Authenticate();

        // Subscribe to events for managing disconnections and stopping the server
        Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        Unity.Netcode.NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Creates a multiplayer relay and generates a join code
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

            if (echoChannel != null) echoChannel.SetChannelCode(joinCode);

            // Example usage in another script
            Unity.Netcode.NetworkManager.Singleton.StartHost();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating relay: {e.Message}");
        }
    }

    // Joins an existing session using the given join code
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

            if (echoChannel != null) echoChannel.SetChannelCode(joinCode);

            Unity.Netcode.NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error joining session: {e.Message}");
        }
    }

    // Handles client disconnection
    private void OnClientDisconnected(ulong clientId)
    {
        // Check if the local client is disconnected
        if (Unity.Netcode.NetworkManager.Singleton.IsClient && clientId == Unity.Netcode.NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Client disconnected from the host. Displaying main menu.");

            if (mainMenuUI != null)
            {
                mainMenuUI.UIVisibility(); // Show the main menu
            }
            else
            {
                Debug.LogError("MainMenuUIDocument is not assigned.");
            }
        }
    }

    // Handles server stop event
    private void OnServerStopped(bool obj)
    {
        // If the server is stopped, show the main menu
        Debug.Log("Server has stopped. Redirecting to the main menu.");

        if (mainMenuUI != null)
        {
            mainMenuUI.UIVisibility(); // Show the main menu
        }
        else
        {
            Debug.LogError("MainMenuUIDocument is not assigned.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid errors
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            Unity.Netcode.NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }
}
