using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Apiv : MonoBehaviour
{
    public string apiUpdateZoneUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/update-zone";

    void Start()
    {
        // Pas besoin de stocker le token en variable de classe
    }

    public void SendZoneUpdate(string zoneName)
    {
        string authToken = PlayerPrefs.GetString("authToken", "");

        if (string.IsNullOrEmpty(authToken))
        {
            Debug.Log("Token d'authentification introuvable !");
            return;
        }

        // Lancer la requ�te API avec le token r�cup�r�
        StartCoroutine(SendZoneRequest(zoneName, authToken));
    }

    IEnumerator SendZoneRequest(string zoneName, string authToken)
    {
        // Cr�ation des donn�es JSON
        ZoneData zoneData = new ZoneData(zoneName, authToken);
        string jsonData = JsonUtility.ToJson(zoneData);

        // Cr�ation de la requ�te POST
        UnityWebRequest request = new UnityWebRequest(apiUpdateZoneUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envoyer la requ�te
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Zone mise � jour avec succ�s : " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("URL de la requ�te: " + apiUpdateZoneUrl);
            Debug.Log("Donn�es envoy�es: " + jsonData);
            Debug.Log("Erreur lors de la mise � jour de la zone : " + request.error);
        }
    }

    // Classe pour les donn�es de la requ�te
    [System.Serializable]
    public class ZoneData
    {
        public string token;
        public string zoneAtteinte;

        public ZoneData(string zoneAtteinte, string token)
        {
            this.zoneAtteinte = zoneAtteinte;
            this.token = token;
        }
    }
}