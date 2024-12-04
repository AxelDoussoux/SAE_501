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
        // R�cup�re le login et le mot de passe de l'utilisateur
        string userLogin = login.text;
        string userPassword = password.text;

        // Appel de la fonction qui g�re la requ�te d'authentification
        StartCoroutine(LoginUser(userLogin, userPassword));
    }

    // Coroutine pour envoyer la requ�te d'authentification
    IEnumerator LoginUser(string userLogin, string userPassword)
    {
        // Pr�pare les donn�es pour la requ�te JSON
        var loginData = new
        {
            login = userLogin,
            password = userPassword
        };

        string json = JsonUtility.ToJson(loginData);

        // Cr�e la requ�te HTTP POST
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envoie la requ�te et attend la r�ponse
        yield return request.SendWebRequest();

        // V�rifie les erreurs
        if (request.result != UnityWebRequest.Result.Success)
        {
            statusText.text = "Error: " + request.error;
        }
        else
        {
            // Si la requ�te r�ussit, affiche la r�ponse (par exemple, un token)
            string response = request.downloadHandler.text;

            // Ici, tu devras probablement extraire le token de la r�ponse
            // Supposons que la r�ponse contient un champ "token"
            // Exemple de r�ponse : { "token": "jwt_token_value" }

            // Tu peux d�s�rialiser la r�ponse pour extraire le token (ceci est un exemple simple)
            var responseData = JsonUtility.FromJson<ResponseData>(response);

            if (responseData != null && !string.IsNullOrEmpty(responseData.token))
            {
                statusText.text = "Connexion r�ussie ! Token: " + responseData.token;
                // Sauvegarder le token pour l'utiliser dans les futures requ�tes
                PlayerPrefs.SetString("auth_token", responseData.token);
                PlayerPrefs.Save();
            }
            else
            {
                statusText.text = "�chec de la connexion. V�rifie tes identifiants.";
            }
        }
    }

    // Structure pour la d�s�rialisation de la r�ponse JSON
    [System.Serializable]
    public class ResponseData
    {
        public string token;
    }
}
