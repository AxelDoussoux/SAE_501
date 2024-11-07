using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerJumpInitializer : NetworkBehaviour
    {
        [SerializeField]
        private float hostJumpForce = 100f;
        [SerializeField]
        private float clientJumpForce = 15f;

        private PlayerMotor _playerMotor;

        private void Awake()
        {
            _playerMotor = GetComponent<PlayerMotor>();

            if (Unity.Netcode.NetworkManager.Singleton.IsHost)
            {
                _playerMotor.jumpForce = hostJumpForce;
            }
            else if (Unity.Netcode.NetworkManager.Singleton.IsClient)
            {
                _playerMotor.jumpForce = clientJumpForce;
            }
        }
    }

}