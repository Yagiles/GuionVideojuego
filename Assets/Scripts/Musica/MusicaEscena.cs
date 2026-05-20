using UnityEngine;

public class MusicaEscena : MonoBehaviour
{
    public AudioClip musica;
    public bool loop = true;

    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ReproducirMusica(musica, loop);
        }
    }
}