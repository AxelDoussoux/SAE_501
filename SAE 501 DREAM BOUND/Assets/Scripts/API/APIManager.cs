using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public string apiUpdateZoneUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/update-zone";
    private string authToken; 

    void Start()
    {
        // Retrieve the saved token (or pass it to this instance)
        authToken = PlayerPrefs.GetString("authToken", "");
    }

    public void SendZoneUpdate(string zoneName)
    {
        if (string.IsNullOrEmpty(authToken))
        {
            Debug.LogError("Authentication token not found!");
            return;
        }

        // Launch the API request
        StartCoroutine(SendZoneRequest(zoneName));
    }

    IEnumerator SendZoneRequest(string zoneName)
    {
        // Creation of the JSON data to be sent (as in the curl request)
        ZoneData zoneData = new ZoneData(zoneName, authToken);
        string jsonData = JsonUtility.ToJson(zoneData);

        // Creating a POST request
        UnityWebRequest request = new UnityWebRequest(apiUpdateZoneUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Area successfully updated : " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("URL of the request: " + apiUpdateZoneUrl);
            Debug.Log("Data sent: " + jsonData);

            Debug.LogError("Error updating zone : " + request.error);
        }
    }

    // Class for query data
    [System.Serializable]
    public class ZoneData
    {
        public string token;
        public string zoneReached;

        public ZoneData(string zoneReached, string token)
        {
            this.zoneReached = zoneReached;
            this.token = token;
        }
    }
}
