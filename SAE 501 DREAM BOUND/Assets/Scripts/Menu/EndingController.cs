using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    [SerializeField] private string nextSceneName; // Name of the next scene to load after video finishes

    // Called when the script is initialized
    private void Start()
    {
        // If videoPlayer is not assigned, get the VideoPlayer component attached to the GameObject
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Subscribe to the event when the video finishes playing
        videoPlayer.loopPointReached += OnVideoFinished;

        // Start playing the video
        PlayVideo();
    }

    // Plays the video
    private void PlayVideo()
    {
        videoPlayer.Play();
    }

    // Called when the video finishes playing
    private void OnVideoFinished(VideoPlayer vp)
    {
        // If a valid next scene name is provided, load the next scene
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
