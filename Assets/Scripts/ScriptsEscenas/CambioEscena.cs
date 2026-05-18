using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public string nombreSiguienteEscena;

    [Header("Spawn")]
    public string nombreSpawnDestino;

    [Header("Guia")]
    public bool modoVueltaAlEntrar = false;

    [Header("Condicion dialogo")]
    public bool requiereDialogoPrevio = false;
    public DialogoData dialogoNecesario;

    [Header("Fundido")]
    public CanvasGroup pantallaNegra;
    public float duracionFundido = 1f;

    private bool cambiandoEscena = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cambiandoEscena) return;

        if (collision.CompareTag("Player"))
        {
            if (requiereDialogoPrevio)
            {
                if (EstadoDialogos.instancia == null)
                {
                    Debug.LogWarning("No existe EstadoDialogos");
                    return;
                }

                if (dialogoNecesario == null)
                {
                    Debug.LogWarning("Falta asignar el DialogoData necesario");
                    return;
                }

                if (!EstadoDialogos.instancia.HaHabladoCon(dialogoNecesario.name))
                {
                    Debug.Log("Todavia no se ha completado el dialogo necesario: " + dialogoNecesario.name);
                    return;
                }
            }

            StartCoroutine(CambiarEscena());
        }
    }

    private IEnumerator CambiarEscena()
    {
        cambiandoEscena = true;

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