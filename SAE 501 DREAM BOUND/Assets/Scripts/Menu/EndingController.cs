using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName; 
    private void Start()
    {
 
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();


        videoPlayer.loopPointReached += OnVideoFinished;

        PlayVideo();
    }

    private void PlayVideo()
    {
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}