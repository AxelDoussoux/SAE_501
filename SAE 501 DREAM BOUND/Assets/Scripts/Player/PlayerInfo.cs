using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class PlayerInfo : NetworkBehaviour
    {
        [SerializeField] private GameObject[] spawnPoints;
        [SerializeField] private bool havehammer = false;
        [SerializeField] private bool haveSpeedShoes = false;
        public Transform SpawnPoint=> spawnPoints[0].transform;
        public bool HaveHammer => havehammer;
        public bool HaveSpeedShoes => haveSpeedShoes;

        public void EnabledHammer()
        {
            havehammer = true;
        }
        public void EnabledSpeedShoes()
        {
            haveSpeedShoes = true;
        }
    }
}
