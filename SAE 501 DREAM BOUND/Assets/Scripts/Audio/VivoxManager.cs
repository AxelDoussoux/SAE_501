using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Vivox;
using Unity.Services.Core;
using Unity.Services.Authentication;

namespace TomAg
{
    public class VivoxManager : NetworkBehaviour
    {
        [SerializeField] private string channelName = "GameVoiceChannel";
        private string playerName;

        async void Start()
        {
            if (!IsOwner)
            {
                Debug.LogWarning("VivoxManager is only initialized by the owner.");
                return;
            }

            // Initialize Unity Services
            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                Debug.Log("Unity Services initialized and authenticated.");

                // Generate a unique player name using LocalClientId
                playerName = Unity.Netcode.NetworkManager.Singleton.LocalClientId.ToString();

                // Connect the player
                await ConnectPlayer();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Initialization failed: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task ConnectPlayer()
        {
            try
            {
                var loginOptions = new LoginOptions
                {
                    DisplayName = playerName
                };

                await VivoxService.Instance.LoginAsync(loginOptions);
                Debug.Log("Player logged into Vivox successfully.");

                // Join the voice channel
                await JoinVoiceChannel();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error connecting player to Vivox: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task JoinVoiceChannel()
        {
            try
            {
                var joinChannelOptions = new ChannelJoinOptions
                {
                    ChannelName = channelName,
                    AudioEnabled = true
                };

                await VivoxService.Instance.JoinGroupChannelAsync(joinChannelOptions);
                Debug.Log("Player joined the Vivox voice channel successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to join Vivox channel: {ex.Message}");
            }
        }


        private async void OnDestroy()
        {
            try
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
                await VivoxService.Instance.LogoutAsync();
                Debug.Log("Vivox service cleaned up.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during cleanup: {ex.Message}");
            }
        }
    }
}
