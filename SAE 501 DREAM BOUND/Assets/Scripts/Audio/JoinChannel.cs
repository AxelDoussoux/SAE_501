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
    // Utilisation de FixedString64Bytes pour la compatibilité Netcode
    private NetworkVariable<FixedString64Bytes> playerGuid =
        new NetworkVariable<FixedString64Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private string channelCode;

    // Appelé lors de la synchronisation du NetworkObject
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // Générer et assigner un GUID unique
            var generatedGuid = GenerateUniqueUserID();
            playerGuid.Value = generatedGuid;
            Debug.Log($"Generated and set GUID: {playerGuid.Value}");
        }

        // Écouter les mises à jour de la variable
        playerGuid.OnValueChanged += OnGuidChanged;
    }

    // Méthode pour gérer les changements de GUID
    private void OnGuidChanged(FixedString64Bytes oldGuid, FixedString64Bytes newGuid)
    {
        Debug.Log($"GUID updated for this player: {newGuid}");
    }

    // Définit le code du canal
    public void SetChannelCode(string joinCode)
    {
        channelCode = joinCode;
        Debug.Log($"Channel code set to: {channelCode}");
        JoinChannelIfReady();
    }

    // Générer un ID utilisateur unique basé sur un GUID
    private FixedString64Bytes GenerateUniqueUserID()
    {
        return new FixedString64Bytes("Player_" + Guid.NewGuid().ToString());
    }

    // Méthode pour rejoindre un canal si les services sont prêts
    private async void JoinChannelIfReady()
    {
        try
        {

            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized.");


            await VivoxService.Instance.InitializeAsync();
            Debug.Log("Vivox Service initialized.");

            // Vérifie si l'utilisateur est déjà connecté
            if (!VivoxService.Instance.IsLoggedIn)
            {
                LoginOptions loginOptions = new LoginOptions
                {
                    PlayerId = playerGuid.Value.ToString()     // Utiliser le GUID comme identifiant unique
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

    // Méthode pour quitter le canal
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
        // Désabonnement pour éviter les erreurs
        playerGuid.OnValueChanged -= OnGuidChanged;
    }
}
