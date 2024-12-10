using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class Signin : MonoBehaviour
{
    // Champs pour r�cup�rer le login et le mot de passe depuis l'interface utilisateur
    public TMP_InputField login;
    public TMP_InputField password;

    // L'URL de ton API pour la cr�ation de compte
    private string apiUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/logins";

    // M�thode pour cr�er un compte
    public void CreateAccount()
    {
        // V�rifier que le login et le mot de passe ne sont pas vides
        if (string.IsNullOrEmpty(login.text) || string.IsNullOrEmpty(password.text))
        {
            Debug.Log("Login ou mot de passe manquant");
            return;
        }

        // Cr�er un objet JSON pour envoyer les donn�es � l'API
        string jsonData = "{\"login\": \"" + login.text + "\", \"password\": \"" + password.text + "\", \"roles\": [\"ROLE_USER\"]}";

        // D�marrer la coroutine pour envoyer la requ�te HTTP
        StartCoroutine(SendCreateAccountRequest(jsonData));
    }

    // Coroutine pour envoyer la requ�te HTTP � l'API
    IEnumerator SendCreateAccountRequest(string jsonData)
    {
        // Cr�er une requ�te HTTP POST vers l'API
        UnityWebRequest www = new UnityWebRequest(apiUrl, "POST");

        // Convertir le JSON en bytes et l'envoyer avec la requ�te
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();

        // D�finir l'en-t�te Content-Type comme application/ld+json
        www.SetRequestHeader("Content-Type", "application/ld+json");

        // Attendre la r�ponse de l'API
        yield return www.SendWebRequest();

        // V�rifier la r�ponse de la requ�te
        if (www.result == UnityWebRequest.Result.Success)
        {
            // R�ponse r�ussie, afficher la r�ponse dans la console
            Debug.Log("Compte cr�� avec succ�s ! R�ponse: " + www.downloadHandler.text);
        }
        else
        {
            // Si une erreur s'est produite, afficher l'erreur dans la console
            Debug.LogError("Erreur lors de la cr�ation du compte: " + www.error);
        }
    }
}
