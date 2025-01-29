using Unity.Netcode;
using UnityEngine;
using DialogueEditor;

namespace TomAg
{
    public class PlayerInfo : NetworkBehaviour
    {
        [SerializeField] private GameObject[] spawnPoints;
        [SerializeField] private bool havehammer = false;
        [SerializeField] private bool haveSpeedShoes = false;

        [SerializeField] public bool canTakeHammer;
        [SerializeField] public bool canTakeSpeedShoes;
        [SerializeField] public bool canInteractWithButtons = true;

        [SerializeField] private GameObject _hammer;
        [SerializeField] private GameObject _speedShoes1;
        [SerializeField] private GameObject _speedShoes2;

        public Transform SpawnPoint => spawnPoints[0].transform;
        public bool HaveHammer => havehammer;
        public bool HaveSpeedShoes => haveSpeedShoes;

        public void EnabledHammer()
        {
            // Enable the hammer and set it to active
            havehammer = true;
            _hammer.SetActive(true);
        }

        public void EnabledSpeedShoes()
        {
            // Enable speed shoes and set them to active
            haveSpeedShoes = true;
            _speedShoes1.SetActive(true);
            _speedShoes2.SetActive(true);
        }
    }
}
