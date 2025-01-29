using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CinematicController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    [SerializeField] private UIDocument uiDocument; // Reference to the UIDocument for the UI elements

    [Header("Settings")]
    [SerializeField] private bool startPlayingOnLoad = true; // Determines if the video should start playing automatically on load

    private bool videoStarted = false; // Tracks if the video has started
    private bool videoFinished = false; // Tracks if the video has finished
    private Label skipPromptLabel; // UI element to display the skip prompt

    // Called when the script is enabled
    private void OnEnable()
    {
        // Get the root element of the UI Document
        var root = uiDocument.rootVisualElement;

        // Find the skip prompt label
        skipPromptLabel = root.Q<Label>("skip-prompt");

        // If the skip prompt label exists, set its text to instruct the user on how to skip the video
        if (skipPromptLabel != null)
            skipPromptLabel.text = "Appuyer sur la barre espace pour passer la vidéo";
    }

    // Called at the start of the script
    private void Start()
    {
        // Setup the video player if it hasn't been assigned already
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Add listeners for when the video finishes playing
        videoPlayer.loopPointReached += OnVideoFinished;

        // Optionally start the video automatically if the setting is true
        if (startPlayingOnLoad)
            StartVideo();
    }

    // Called once per frame
    private void Update()
    {
        // Check if the video has started, is not finished, and the user pressed the "Space" key to skip
        if (videoStarted && !videoFinished && Input.GetKeyDown(KeyCode.Space))
        {
            SkipVideo();
        }
    }

    // Starts the video
    public void StartVideo()
    {
        videoStarted = true; // Mark that the video has started
        videoPlayer.Play();  // Play the video
    }

    // Skips the video by stopping it and immediately loading the next scene
    private void SkipVideo()
    {
        videoPlayer.Stop(); // Stop the video
        LoadNextScene();    // Load the next scene
    }

    // Called when the video finishes playing
    private void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true; // Mark that the video is finished
        LoadNextScene();     // Load the next scene
    }

    // Loads the next scene (hardcoded to "Fonctionnement")
    private void LoadNextScene()
    {
        // If the scene name is valid, load it
        if (!string.IsNullOrEmpty("Fonctionnement"))
            SceneManager.LoadScene("Fonctionnement");
    }
}
