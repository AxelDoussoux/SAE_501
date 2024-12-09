using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class PlayerInfo : NetworkBehaviour
    {
        [SerializeField] private GameObject[] spawnPoints;



        public Transform SpawnPoint=> spawnPoints[0].transform;
    }
}
