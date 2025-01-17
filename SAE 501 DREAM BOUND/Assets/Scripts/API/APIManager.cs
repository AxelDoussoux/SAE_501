using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public string apiUpdateZoneUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/update-zone";
    private string authToken; // Le token d'authentification du joueur

    void Start()
    {
        // R�cup�rer le token sauvegard� (ou le passer � cette instance)
        authToken = PlayerPrefs.GetString("authToken", "");
    }

    public void SendZoneUpdate(string zoneName)
    {
        if (string.IsNullOrEmpty(authToken))
        {
            Debug.LogError("Token d'authentification introuvable !");
            return;
        }

        // Lancer la requ�te API
        StartCoroutine(SendZoneRequest(zoneName));
    }

    IEnumerator SendZoneRequest(string zoneName)
    {
        // Cr�ation des donn�es JSON � envoyer (comme dans la requ�te curl)
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

            Debug.LogError("Erreur lors de la mise � jour de la zone : " + request.error);
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
