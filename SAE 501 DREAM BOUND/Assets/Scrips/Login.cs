using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // Pour le bouton Unity classique
using TMPro; // Pour TextMeshPro

public class Login : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField loginInputField;  // Champ pour le login (TextMeshPro)
    public TMP_InputField passwordInputField;  // Champ pour le mot de passe (TextMeshPro)
    public TMP_Text feedbackText;  // Texte pour afficher les erreurs ou succès (TextMeshPro)
    public Button validateButton;  // Bouton classique (UnityEngine.UI)

    [Header("API Configuration")]
    public string apiLoginUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/login";

    private bool isRequestInProgress = false; // Pour éviter des requêtes multiples

    void Start()
    {
        // Associer le bouton au gestionnaire d'événements
        validateButton.onClick.AddListener(OnValidateButtonClick);
    }

    public void OnValidateButtonClick()
    {
        if (isRequestInProgress)
        {
            feedbackText.text = "Requête en cours, veuillez patienter...";
            return;
        }

        string login = loginInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Le login ou le mot de passe est vide.";
            return;
        }

        // Lancer la requête API
        StartCoroutine(SendLoginRequest(login, password));
    }

    IEnumerator SendLoginRequest(string login, string password)
    {
        isRequestInProgress = true;
        validateButton.interactable = false; // Désactiver le bouton pendant la requête
        feedbackText.text = "Connexion en cours...";

        // Création des données JSON pour la requête
        string jsonData = JsonUtility.ToJson(new LoginData(login, password));

        // Création de la requête HTTP POST
        UnityWebRequest request = new UnityWebRequest(apiLoginUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envoyer la requête et attendre la réponse
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Connexion réussie : " + request.downloadHandler.text);
            feedbackText.text = "Connexion réussie.";
            HandleLoginResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Erreur lors de la connexion : " + request.error);
            feedbackText.text = "Erreur : " + request.error;
        }

        // Réinitialiser l'état du bouton
        validateButton.interactable = true;
        isRequestInProgress = false;
    }

    void HandleLoginResponse(string jsonResponse)
    {
        // Traiter la réponse de l'API
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

        if (!string.IsNullOrEmpty(response.token))
        {
            Debug.Log("Token reçu : " + response.token);
            feedbackText.text = "Connexion réussie. Token : " + response.token;

            // TODO: Stocker le token ou rediriger vers une autre scène

            PlayerPrefs.SetString("authToken", response.token);
            PlayerPrefs.Save();
            Debug.Log("Token sauvegardé : " + PlayerPrefs.GetString("authToken"));
        }
        else
        {
            Debug.LogError("Erreur : " + response.error);
            feedbackText.text = "Erreur : " + response.error;
        }
    }

    // Classe pour les données de connexion
    [System.Serializable]
    public class LoginData
    {
        public string login;
        public string password;

        public LoginData(string login, string password)
        {
            this.login = login;
            this.password = password;
        }
    }

    // Classe pour la réponse de l'API
    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public string expires_at;
        public string error;
    }
}
