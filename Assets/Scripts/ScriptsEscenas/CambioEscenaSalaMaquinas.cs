using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscenaSalaMaquinas : MonoBehaviour
{
    [Header("Cambio de escena")]
    public string nombreSiguienteEscena;

    [Header("Spawn")]
    public string nombreSpawnDestino;

    [Header("Guia")]
    public bool modoVueltaAlEntrar = false;

    [Header("Fundido")]
    public CanvasGroup pantallaNegra;

    private bool cambiando = false;

    private void OnEnable()
    {
        if (!cambiando)
        {
            StartCoroutine(CambiarEscena());
        }
    }

    private IEnumerator CambiarEscena()
    {
        cambiando = true;

        DatosCambioEscena.spawnDestino = nombreSpawnDestino;
        DatosCambioEscena.modoVuelta = modoVueltaAlEntrar;

        if (pantallaNegra != null)
        {
            pantallaNegra.gameObject.SetActive(true);
            pantallaNegra.alpha = 1f;
        }

        yield return null;

        SceneManager.LoadScene(nombreSiguienteEscena);
    }
}