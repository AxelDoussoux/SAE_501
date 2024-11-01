using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    private void Start()
    {
        if (IsOwner) 
        {
            CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                virtualCamera.Follow = transform; 
                virtualCamera.LookAt = transform; 
            }
        }
    }
}
