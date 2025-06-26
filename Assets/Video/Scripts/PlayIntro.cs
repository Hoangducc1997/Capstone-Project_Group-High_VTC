// Attach vào VideoPlayer object
using UnityEngine;
using UnityEngine.Video;

public class PlayIntro : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "MainMenu";

    void Start()
    {
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}
