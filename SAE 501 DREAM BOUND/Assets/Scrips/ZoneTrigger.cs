using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public string zoneName = "zone 1"; // Nom de la zone

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Le joueur est entré dans la zone : " + zoneName);
            GameObject.FindObjectOfType<Apiv>().SendZoneUpdate(zoneName);
        }
    }
}

