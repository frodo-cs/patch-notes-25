using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("1_entry_bay");
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
}
