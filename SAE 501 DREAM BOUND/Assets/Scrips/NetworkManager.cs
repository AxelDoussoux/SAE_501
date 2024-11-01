using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    public UnityTransport transport;
    public TMP_InputField joinCodeInputField;

    async void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("UnityTransport not found. Please ensure it's attached to the Network Manager.");
            return;
        }

        await Authenticate();
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateMultiplayerRelay()
    {
        try
        {
            if (transport == null)
            {
                Debug.LogError("Transport not initialized.");
                return;
            }

            Allocation a = await RelayService.Instance.CreateAllocationAsync(2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
            if (joinCodeInputField != null)
            {
                joinCodeInputField.text = joinCode;
            }
            else
            {
                Debug.LogError("Join code input field is not assigned.");
            }

            transport.SetRelayServerData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            Unity.Netcode.NetworkManager.Singleton.StartHost();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating relay: {e.Message}");
        }
    }

    public void StartHost()
    {
        if (Unity.Netcode.NetworkManager.Singleton != null)
        {
            Unity.Netcode.NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.LogError("NetworkManager Singleton not initialized.");
        }
    }

    public async void JoinSession()
    {
        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCodeInputField.text);
        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
        Unity.Netcode.NetworkManager.Singleton.StartClient();
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