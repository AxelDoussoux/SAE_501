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
    private Button boutonJouerSansConnexion;
    private Button boutonInscription; // Nouveau bouton pour l'inscription
    private Label feedbackLabel; // Champ de texte pour afficher les messages de feedback

    [Header("API Configuration")]
    public string apiLoginUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/login";
    public string apiInscriptionUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/inscription"; // URL d'inscription

    private bool isRequestInProgress = false; // Évite les multiples requêtes

    void Start()
    {
        // Récupérer les éléments UI depuis le document
        var root = uiDocument.rootVisualElement;

        loginInputField = root.Q<TextField>("loginInput");
        passwordInputField = root.Q<TextField>("passwordInput");
        boutonConnexion = root.Q<Button>("boutonconnexion");
        boutonJouerSansConnexion = root.Q<Button>("jouersansconnexion");
        boutonInscription = root.Q<Button>("boutoninscription"); // Récupération du bouton d'inscription
        feedbackLabel = root.Q<Label>("feedback");

        // Associer le clic du bouton à l'action de validation
        boutonConnexion.clicked += OnValidateButtonClick;

        // Associer le clic du bouton pour jouer sans connexion
        boutonJouerSansConnexion.clicked += OnJouerSansConnexionClick;

        // Associer le clic du bouton pour l'inscription
        boutonInscription.clicked += OnInscriptionClick;
    }

    public void OnValidateButtonClick()
    {
        if (isRequestInProgress)
        {
            Debug.Log("Requête en cours, veuillez patienter...");
            return;
        }

        string login = loginInputField.value;
        string password = passwordInputField.value;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Le login ou le mot de passe est vide.");
            return;
        }

        // Lancer la requête API
        StartCoroutine(SendLoginRequest(login, password));
    }

    IEnumerator SendLoginRequest(string login, string password)
    {
        isRequestInProgress = true;
        boutonConnexion.SetEnabled(false); // Désactiver le bouton pendant la requête
        feedbackLabel.text = ""; // Réinitialiser le message précédent

        Debug.Log("Connexion en cours...");

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
            HandleLoginResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Erreur lors de la connexion : " + request.error);

            // Afficher un message d'erreur dans le label
            feedbackLabel.text = "Connexion échouée. Veuillez réessayer"; // Afficher le message d'erreur

            // Réactiver le bouton pour permettre à l'utilisateur de réessayer
            boutonConnexion.SetEnabled(true);
        }

        // Réinitialiser l'état de la requête
        isRequestInProgress = false;
    }


    void HandleLoginResponse(string jsonResponse)
    {
        // Traiter la réponse de l'API
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

        if (!string.IsNullOrEmpty(response.token))
        {
            Debug.Log("Token reçu : " + response.token);

            // Sauvegarder le token localement
            PlayerPrefs.SetString("authToken", response.token);
            PlayerPrefs.Save();

            Debug.Log("Token sauvegardé avec succès.");

            // Redirection vers la scène "Fonctionnement"
            SceneManager.LoadScene("Fonctionnement");
        }
        else
        {
            Debug.LogError("Erreur : " + response.error);

            // Afficher un message d'erreur dans le label
            if (feedbackLabel != null)
            {
                feedbackLabel.text = "Connexion échouée. Veuillez réessayer"; // Afficher le message d'erreur
            }
            else
            {
                Debug.LogWarning("Le label feedback est nul.");
            }
        }
    }

    // Fonction pour jouer sans connexion
    public void OnJouerSansConnexionClick()
    {
        Debug.Log("Jouer sans connexion sélectionné. Chargement de la scène Fonctionnement...");
        SceneManager.LoadScene("Fonctionnement");
    }

    // Fonction pour ouvrir la page d'inscription
    public void OnInscriptionClick()
    {
        Debug.Log("Inscription sélectionnée. Ouverture de l'URL d'inscription...");
        Application.OpenURL(apiInscriptionUrl); // Ouvrir l'URL d'inscription dans le navigateur
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
