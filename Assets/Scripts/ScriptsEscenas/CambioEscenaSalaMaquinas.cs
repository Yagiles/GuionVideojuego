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
    public float duracionFundido = 1f;

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

        float tiempo = 0f;

        while (tiempo < duracionFundido)
        {
            tiempo += Time.deltaTime;

            if (pantallaNegra != null)
            {
                pantallaNegra.alpha = tiempo / duracionFundido;
            }

            yield return null;
        }

        SceneManager.LoadScene(nombreSiguienteEscena);
    }
}