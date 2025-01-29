using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PaintingManager : MonoBehaviour
{
    public string apiGetPaintingUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/get-painting";
    public Material[] paintingMaterials; // Array of 3 materials
    public float checkInterval = 5f; // Check every 5 seconds

    private Renderer tableauRenderer; // Renderer of the quad
    private int currentPaintingId = 0;

    void Start()
    {
        tableauRenderer = GetComponent<Renderer>();
        if (paintingMaterials.Length != 3)
        {
            Debug.LogError("Exactly 3 materials are required!");
            return;
        }

        // Start periodic checking
        StartCoroutine(CheckPaintingIdRoutine());
    }

    IEnumerator CheckPaintingIdRoutine()
    {
        while (true)
        {
            string authToken = PlayerPrefs.GetString("authToken", "");
            if (!string.IsNullOrEmpty(authToken))
            {
                yield return StartCoroutine(GetPaintingId(authToken));
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    IEnumerator GetPaintingId(string authToken)
    {
        if (string.IsNullOrEmpty(authToken))
        {
            // Case where the player is not logged in
            Debug.LogWarning("No authentication token found. Using default image.");
            currentPaintingId = 1; // Use default image (e.g., the first one)
            UpdatePainting();
            yield break;
        }

        // Case where the player is logged in
        PaintingData paintingData = new PaintingData(authToken);
        string jsonData = JsonUtility.ToJson(paintingData);

        UnityWebRequest request = new UnityWebRequest(apiGetPaintingUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            PaintingResponse paintingResponse = JsonUtility.FromJson<PaintingResponse>(response);

            if (paintingResponse.paintingid != currentPaintingId)
            {
                currentPaintingId = paintingResponse.paintingid;
                UpdatePainting();
            }
        }
        else
        {
            Debug.LogError($"Error retrieving painting ID: {request.error}");
            Debug.LogError("Server response: " + request.downloadHandler.text);
        }
    }

    void UpdatePainting()
    {
        // Update the displayed painting based on the received ID
        if (currentPaintingId >= 1 && currentPaintingId <= 3)
        {
            tableauRenderer.material = paintingMaterials[currentPaintingId - 1];
            Debug.Log("Painting updated with image " + currentPaintingId);
        }
    }

    [System.Serializable]
    private class PaintingData
    {
        public string token;

        public PaintingData(string token)
        {
            this.token = token;
        }
    }

    [System.Serializable]
    private class PaintingResponse
    {
        public int paintingid;
    }
}