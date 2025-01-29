using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class ZoneTrigger : MonoBehaviour
{
    public string zoneName = "zone 1"; // Name of the zone

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the zone: " + zoneName);
            GameObject.FindObjectOfType<Apiv>().SendZoneUpdate(zoneName);
        }
    }
}