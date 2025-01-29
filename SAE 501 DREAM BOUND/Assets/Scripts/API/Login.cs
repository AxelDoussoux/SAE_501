using System.Collections;
using UnityEngine;
using UnityEngine.UIElements; 
using UnityEngine.SceneManagement; 
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    [Header("UI Toolkit Elements")]
    public UIDocument uiDocument; 

    private TextField loginInputField; 
    private TextField passwordInputField; 
    private Button boutonConnexion; 
    private Button boutonJouerSansConnexion;
    private Button boutonInscription; 
    private Label feedbackLabel; 

    [Header("API Configuration")]
    public string apiLoginUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/api/login";
    public string apiInscriptionUrl = "https://scep.prox.dsi.uca.fr/vm-mmi03-web-31/api/public/inscription"; 

    private bool isRequestInProgress = false; // Prevents multiple requests

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        loginInputField = root.Q<TextField>("loginInput");
        passwordInputField = root.Q<TextField>("passwordInput");

        // Hide the placeholder text "Password"
        passwordInputField.label = "";

        // Enable password masking while typing
        passwordInputField.RegisterCallback<InputEvent>(evt => {
            passwordInputField.isPasswordField = true;
        });

        boutonConnexion = root.Q<Button>("boutonconnexion");
        boutonJouerSansConnexion = root.Q<Button>("jouersansconnexion");
        boutonInscription = root.Q<Button>("boutoninscription");
        feedbackLabel = root.Q<Label>("feedback");

        boutonConnexion.clicked += OnValidateButtonClick;
        boutonJouerSansConnexion.clicked += OnJouerSansConnexionClick;
        boutonInscription.clicked += OnInscriptionClick;
    }

    public void OnValidateButtonClick()
    {
        if (isRequestInProgress)
        {
            Debug.Log("Request in progress, please wait...");
            return;
        }

        string login = loginInputField.value;
        string password = passwordInputField.value;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Login or password is empty.");
            return;
        }

        StartCoroutine(SendLoginRequest(login, password));
    }

    IEnumerator SendLoginRequest(string login, string password)
    {
        isRequestInProgress = true;
        boutonConnexion.SetEnabled(false);
        feedbackLabel.text = "";

        Debug.Log("Logging in...");

        string jsonData = JsonUtility.ToJson(new LoginData(login, password));

        UnityWebRequest request = new UnityWebRequest(apiLoginUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Login successful: " + request.downloadHandler.text);
            HandleLoginResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Login error: " + request.error);
            feedbackLabel.text = "Login failed. Please try again.";
            boutonConnexion.SetEnabled(true);
        }

        isRequestInProgress = false;
    }

    void HandleLoginResponse(string jsonResponse)
    {
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

        if (!string.IsNullOrEmpty(response.token))
        {
            Debug.Log("Token received: " + response.token);
            PlayerPrefs.SetString("authToken", response.token);
            PlayerPrefs.Save();
            Debug.Log("Token saved successfully.");
            SceneManager.LoadScene("Cinematic");
        }
        else
        {
            Debug.LogError("Error: " + response.error);
            if (feedbackLabel != null)
            {
                feedbackLabel.text = "Login failed. Please try again.";
            }
            else
            {
                Debug.LogWarning("Feedback label is null.");
            }
        }
    }

    public void OnJouerSansConnexionClick()
    {
        Debug.Log("Play without login selected. Loading Cinematic scene...");
        SceneManager.LoadScene("Cinematic");
    }

    public void OnInscriptionClick()
    {
        Debug.Log("Registration selected. Opening registration URL...");
        Application.OpenURL(apiInscriptionUrl);
    }

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

    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public string expires_at;
        public string error;
    }
}
