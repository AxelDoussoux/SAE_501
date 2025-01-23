using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PaintingManager : MonoBehaviour
{
    public string apiGetPaintingUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/get-painting";
    public Material[] paintingMaterials; // Tableau des 3 mat�riaux
    public float checkInterval = 5f; // V�rifier toutes les 5 secondes

    private Renderer tableauRenderer; // Le renderer du quad
    private int currentPaintingId = 0;

    void Start()
    {
        tableauRenderer = GetComponent<Renderer>();
        if (paintingMaterials.Length != 3)
        {
            Debug.LogError("Il faut exactement 3 mat�riaux !");
            return;
        }

        // D�marrer la v�rification p�riodique
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
            // Cas o� le joueur n'est pas connect�
            Debug.LogWarning("Pas de token d'authentification trouv�. Utilisation de l'image par d�faut.");
            currentPaintingId = 1; // Utilisation de l'image par d�faut (par exemple, la premi�re)
            UpdatePainting();
            yield break;
        }

        // Cas o� le joueur est connect�
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
            Debug.LogError($"Erreur lors de la r�cup�ration du paintingid : {request.error}");
            Debug.LogError("R�ponse du serveur : " + request.downloadHandler.text);
        }
    }


    void UpdatePainting()
    {
        if (currentPaintingId >= 1 && currentPaintingId <= 3)
        {
            tableauRenderer.material = paintingMaterials[currentPaintingId - 1];
            Debug.Log("Tableau mis � jour avec l'image " + currentPaintingId);
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