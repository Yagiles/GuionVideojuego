using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CambiarEscenaTrasVideo : MonoBehaviour
{
    public string nombreEscena;

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoTerminado;
    }

    void VideoTerminado(VideoPlayer vp)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}