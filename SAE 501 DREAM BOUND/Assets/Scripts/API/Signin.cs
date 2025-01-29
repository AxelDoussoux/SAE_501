using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class Signin : MonoBehaviour
{
    public TMP_InputField login;
    public TMP_InputField password;

    private string apiUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/logins";

    // Method to create an account
    public void CreateAccount()
    {
        if (string.IsNullOrEmpty(login.text) || string.IsNullOrEmpty(password.text))
        {
            Debug.Log("Login or password missing");
            return;
        }

        string jsonData = "{\"login\": \"" + login.text + "\", \"password\": \"" + password.text + "\", \"roles\": [\"ROLE_USER\"]}";

        StartCoroutine(SendCreateAccountRequest(jsonData));
    }

    // Coroutine to send an HTTP request to the API
    IEnumerator SendCreateAccountRequest(string jsonData)
    {
        UnityWebRequest www = new UnityWebRequest(apiUrl, "POST");

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/ld+json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Account successfully created! Response: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error creating account: " + www.error);
        }
    }
}
