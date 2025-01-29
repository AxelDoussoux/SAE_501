using UnityEngine;
using Unity.Services.Vivox;
using Unity.Services.Core;
using Unity.Collections;
using Unity.Netcode;
using System;
#if AUTH_PACKAGE_PRESENT
using Unity.Services.Authentication;
#endif

public class JoinChannel : NetworkBehaviour
{
    private NetworkVariable<FixedString64Bytes> playerGuid =
        new NetworkVariable<FixedString64Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private string channelCode;

    // Called when the NetworkObject is synchronized
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            var generatedGuid = GenerateUniqueUserID();
            playerGuid.Value = generatedGuid;
            Debug.Log($"Generated and set GUID: {playerGuid.Value}");
        }

        playerGuid.OnValueChanged += OnGuidChanged;
    }

    // Handles GUID changes
    private void OnGuidChanged(FixedString64Bytes oldGuid, FixedString64Bytes newGuid)
    {
        Debug.Log($"GUID updated for this player: {newGuid}");
    }

    // Sets the channel code
    public void SetChannelCode(string joinCode)
    {
        channelCode = joinCode;
        Debug.Log($"Channel code set to: {channelCode}");
        JoinChannelIfReady();
    }

    // Generates a unique user ID based on a GUID
    private FixedString64Bytes GenerateUniqueUserID()
    {
        return new FixedString64Bytes("Player_" + Guid.NewGuid().ToString());
    }

    // Joins a channel if services are ready
    private async void JoinChannelIfReady()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized.");

            await VivoxService.Instance.InitializeAsync();
            Debug.Log("Vivox Service initialized.");

            if (!VivoxService.Instance.IsLoggedIn)
            {
                LoginOptions loginOptions = new LoginOptions
                {
                    PlayerId = playerGuid.Value.ToString()
                };

                await VivoxService.Instance.LoginAsync(loginOptions);
                Debug.Log($"Player {playerGuid.Value} logged in successfully.");
            }

            await VivoxService.Instance.JoinGroupChannelAsync(channelCode, ChatCapability.AudioOnly);
            Debug.Log($"Player {playerGuid.Value} joined the channel successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error while joining channel: {ex.Message}");
        }
    }

    // Leaves the channel
    public async void LeaveChannel()
    {
        try
        {
            await VivoxService.Instance.LeaveChannelAsync(channelCode);
            Debug.Log($"Player {playerGuid.Value} left the channel successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error while leaving channel: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        playerGuid.OnValueChanged -= OnGuidChanged;
    }
}
