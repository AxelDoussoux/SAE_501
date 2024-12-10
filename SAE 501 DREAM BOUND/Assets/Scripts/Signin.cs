using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class Signin : MonoBehaviour
{
    // Champs pour récupérer le login et le mot de passe depuis l'interface utilisateur
    public TMP_InputField login;
    public TMP_InputField password;

    // L'URL de ton API pour la création de compte
    private string apiUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/logins";

    // Méthode pour créer un compte
    public void CreateAccount()
    {
        // Vérifier que le login et le mot de passe ne sont pas vides
        if (string.IsNullOrEmpty(login.text) || string.IsNullOrEmpty(password.text))
        {
            Debug.Log("Login ou mot de passe manquant");
            return;
        }

        // Créer un objet JSON pour envoyer les données à l'API
        string jsonData = "{\"login\": \"" + login.text + "\", \"password\": \"" + password.text + "\", \"roles\": [\"ROLE_USER\"]}";

        // Démarrer la coroutine pour envoyer la requête HTTP
        StartCoroutine(SendCreateAccountRequest(jsonData));
    }

    // Coroutine pour envoyer la requête HTTP à l'API
    IEnumerator SendCreateAccountRequest(string jsonData)
    {
        // Créer une requête HTTP POST vers l'API
        UnityWebRequest www = new UnityWebRequest(apiUrl, "POST");

        // Convertir le JSON en bytes et l'envoyer avec la requête
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();

        // Définir l'en-tête Content-Type comme application/ld+json
        www.SetRequestHeader("Content-Type", "application/ld+json");

        // Attendre la réponse de l'API
        yield return www.SendWebRequest();

        // Vérifier la réponse de la requête
        if (www.result == UnityWebRequest.Result.Success)
        {
            // Réponse réussie, afficher la réponse dans la console
            Debug.Log("Compte créé avec succès ! Réponse: " + www.downloadHandler.text);
        }
        else
        {
            // Si une erreur s'est produite, afficher l'erreur dans la console
            Debug.LogError("Erreur lors de la création du compte: " + www.error);
        }
    }
}
