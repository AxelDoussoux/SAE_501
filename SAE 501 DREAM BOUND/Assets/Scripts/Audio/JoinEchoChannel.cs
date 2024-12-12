using UnityEngine;
using Unity.Services.Vivox;
using Unity.Services.Core;
#if AUTH_PACKAGE_PRESENT
using Unity.Services.Authentication;
#endif

/// <summary>
/// Very simple script that does the bare minimum to get a user into an echo channel.
/// When entering playmode the user should be able to quickly hear themselves speaking.
/// Currently this will only run in the editor due some platforms requiring device permissions.
/// Requires that a project has been linked in Edit > Project Settings > Services > Vivox
/// Requires the Authentication package to also be present in the project.
/// </summary>
public class JoinEchoChannel : MonoBehaviour
{
    private string channelCode;

    public void SetChannelCode(string joinCode)
    {
        channelCode = joinCode;
        Debug.Log(channelCode);
        launchChannel();
    }
    async void launchChannel()
    {

        await UnityServices.InitializeAsync();
        await VivoxService.Instance.InitializeAsync();
        await VivoxService.Instance.LoginAsync();
        await VivoxService.Instance.JoinEchoChannelAsync(channelCode, ChatCapability.AudioOnly);

    }
}
