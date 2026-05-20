using UnityEngine;

public class TriggerMusicaTemporal : MonoBehaviour
{
    public AudioClip musicaTemporal;

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activado) return;
        if (!collision.CompareTag("Player")) return;

        activado = true;

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ReproducirTemporal(musicaTemporal);
        }
    }
}