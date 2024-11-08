using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    //[RequireComponent(typeof(PlayerMotor))]
    public class PlayerJumpInitializer : NetworkBehaviour
    {
        [SerializeField]
        private float hostMass = 10f;
        [SerializeField]
        private float clientMass = 5f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (Unity.Netcode.NetworkManager.Singleton.IsHost)
            {
                _rigidbody.mass = hostMass;
            }
            else if (Unity.Netcode.NetworkManager.Singleton.IsClient)
            {
                _rigidbody.mass = clientMass;
            }
        }
    }

}