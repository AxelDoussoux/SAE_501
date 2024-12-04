using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField login;  // Champ de saisie pour le login
    public TMP_InputField password;  // Champ de saisie pour le mot de passe
    public TextMeshProUGUI statusText;  // Text pour afficher le statut de la connexion

    private const string apiUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/login";  // URL de l'API pour la connexion

    // Fonction de connexion
    public void Connect()
    {
        // Récupère le login et le mot de passe de l'utilisateur
        string userLogin = login.text;
        string userPassword = password.text;

        // Appel de la fonction qui gère la requête d'authentification
        StartCoroutine(LoginUser(userLogin, userPassword));
    }

    // Coroutine pour envoyer la requête d'authentification
    IEnumerator LoginUser(string userLogin, string userPassword)
    {
        // Prépare les données pour la requête JSON
        var loginData = new
        {
            login = userLogin,
            password = userPassword
        };

        string json = JsonUtility.ToJson(loginData);

        // Crée la requête HTTP POST
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envoie la requête et attend la réponse
        yield return request.SendWebRequest();

        // Vérifie les erreurs
        if (request.result != UnityWebRequest.Result.Success)
        {
            statusText.text = "Error: " + request.error;
        }
        else
        {
            // Si la requête réussit, affiche la réponse (par exemple, un token)
            string response = request.downloadHandler.text;

            // Ici, tu devras probablement extraire le token de la réponse
            // Supposons que la réponse contient un champ "token"
            // Exemple de réponse : { "token": "jwt_token_value" }

            // Tu peux désérialiser la réponse pour extraire le token (ceci est un exemple simple)
            var responseData = JsonUtility.FromJson<ResponseData>(response);

            if (responseData != null && !string.IsNullOrEmpty(responseData.token))
            {
                statusText.text = "Connexion réussie ! Token: " + responseData.token;
                // Sauvegarder le token pour l'utiliser dans les futures requêtes
                PlayerPrefs.SetString("auth_token", responseData.token);
                PlayerPrefs.Save();
            }
            else
            {
                statusText.text = "Échec de la connexion. Vérifie tes identifiants.";
            }
        }
    }

    // Structure pour la désérialisation de la réponse JSON
    [System.Serializable]
    public class ResponseData
    {
        public string token;
    }
}
