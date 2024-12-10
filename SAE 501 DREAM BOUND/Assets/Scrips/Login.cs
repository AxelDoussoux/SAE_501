using System.Collections;
using UnityEngine;
using UnityEngine.UIElements; // UI Toolkit
using UnityEngine.SceneManagement; // Pour la redirection
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    [Header("UI Toolkit Elements")]
    public UIDocument uiDocument; // Document contenant le canvas UI Toolkit

    private TextField loginInputField; // Champ de texte pour le login
    private TextField passwordInputField; // Champ de texte pour le mot de passe
    private Button boutonConnexion; // Bouton pour valider

    [Header("API Configuration")]
    public string apiLoginUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/login";

    private bool isRequestInProgress = false; // �vite les multiples requ�tes

    void Start()
    {
        // R�cup�rer les �l�ments UI depuis le document
        var root = uiDocument.rootVisualElement;

        loginInputField = root.Q<TextField>("loginInput");
        passwordInputField = root.Q<TextField>("passwordInput");
        boutonConnexion = root.Q<Button>("boutonconnexion");

        // Associer le clic du bouton � l'action de validation
        boutonConnexion.clicked += OnValidateButtonClick;
    }

    public void OnValidateButtonClick()
    {
        if (isRequestInProgress)
        {
            Debug.Log("Requ�te en cours, veuillez patienter...");
            return;
        }

        string login = loginInputField.value;
        string password = passwordInputField.value;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Le login ou le mot de passe est vide.");
            return;
        }

        // Lancer la requ�te API
        StartCoroutine(SendLoginRequest(login, password));
    }

    IEnumerator SendLoginRequest(string login, string password)
    {
        isRequestInProgress = true;
        boutonConnexion.SetEnabled(false); // D�sactiver le bouton pendant la requ�te
        Debug.Log("Connexion en cours...");

        // Cr�ation des donn�es JSON pour la requ�te
        string jsonData = JsonUtility.ToJson(new LoginData(login, password));

        // Cr�ation de la requ�te HTTP POST
        UnityWebRequest request = new UnityWebRequest(apiLoginUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envoyer la requ�te et attendre la r�ponse
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Connexion r�ussie : " + request.downloadHandler.text);
            HandleLoginResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Erreur lors de la connexion : " + request.error);
        }

        // R�initialiser l'�tat du bouton
        boutonConnexion.SetEnabled(true);
        isRequestInProgress = false;
    }

    void HandleLoginResponse(string jsonResponse)
    {
        // Traiter la r�ponse de l'API
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

        if (!string.IsNullOrEmpty(response.token))
        {
            Debug.Log("Token re�u : " + response.token);

            // Sauvegarder le token localement
            PlayerPrefs.SetString("authToken", response.token);
            PlayerPrefs.Save();

            Debug.Log("Token sauvegard� avec succ�s.");

            // Redirection vers la sc�ne "Fonctionnement"
            SceneManager.LoadScene("Fonctionnement");
        }
        else
        {
            Debug.LogError("Erreur : " + response.error);
        }
    }

    // Classe pour les donn�es de connexion
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

    // Classe pour la r�ponse de l'API
    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public string expires_at;
        public string error;
    }
}
