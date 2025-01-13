using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CinematicController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private UIDocument uiDocument;

    [Header("Settings")]
    [SerializeField] private bool startPlayingOnLoad = true;

    private bool videoStarted = false;
    private bool videoFinished = false;
    private Label skipPromptLabel;

    private void OnEnable()
    {
        // Get the root element of the UI Document
        var root = uiDocument.rootVisualElement;

        // Find the skip prompt label
        skipPromptLabel = root.Q<Label>("skip-prompt");

        if (skipPromptLabel != null)
            skipPromptLabel.text = "Appuyer sur la barre espace pour passer la vidéo";
    }

    private void Start()
    {
        // Setup video player
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Add listeners for video events
        videoPlayer.loopPointReached += OnVideoFinished;

        if (startPlayingOnLoad)
            StartVideo();
    }

    private void Update()
    {
        // Check for skip input
        if (videoStarted && !videoFinished && Input.GetKeyDown(KeyCode.Space))
        {
            SkipVideo();
        }
    }

    public void StartVideo()
    {
        videoStarted = true;
        videoPlayer.Play();
    }

    private void SkipVideo()
    {
        videoPlayer.Stop();
        LoadNextScene();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty("Fonctionnement"))
            SceneManager.LoadScene("Fonctionnement");
    }
}