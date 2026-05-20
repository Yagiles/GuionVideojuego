using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;
    private AudioClip musicaAnterior;
    private Coroutine coroutineTemporal;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void ReproducirMusica(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }

        if (coroutineTemporal != null)
        {
            StopCoroutine(coroutineTemporal);
            coroutineTemporal = null;
        }

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void ReproducirTemporal(AudioClip clip)
    {
        if (clip == null) return;

        if (coroutineTemporal != null)
        {
            StopCoroutine(coroutineTemporal);
        }

        coroutineTemporal = StartCoroutine(ReproducirTemporalCoroutine(clip));
    }

    IEnumerator ReproducirTemporalCoroutine(AudioClip clip)
    {
        musicaAnterior = audioSource.clip;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();

        yield return new WaitForSeconds(clip.length);

        if (musicaAnterior != null)
        {
            audioSource.clip = musicaAnterior;
            audioSource.loop = true;
            audioSource.Play();
        }

        coroutineTemporal = null;
    }
}