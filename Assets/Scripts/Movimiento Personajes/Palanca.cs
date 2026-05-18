using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PalancaCaidaEscena : MonoBehaviour
{
    [Header("Condicion dialogo")]
    public DialogoData dialogoNecesario;

    [Header("Palanca")]
    public Transform palancaVisual;
    public float gradosGiro = -60f;
    public float duracionGiro = 0.4f;

    [Header("Suelo que desaparece")]
    public Collider2D colliderSueloADesactivar;

    [Header("Cambio de escena")]
    public string nombreSiguienteEscena;
    public CanvasGroup pantallaNegra;
    public float esperaAntesDeFundido = 0.7f;
    public float duracionFundido = 1f;

    private bool usada = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (usada) return;

        if (!collision.CompareTag("Player")) return;

        if (EstadoDialogos.instancia == null)
        {
            Debug.LogWarning("No existe EstadoDialogos");
            return;
        }

        if (dialogoNecesario == null)
        {
            Debug.LogWarning("Falta asignar DialogoData necesario en la palanca");
            return;
        }

        if (!EstadoDialogos.instancia.HaHabladoCon(dialogoNecesario.name))
        {
            Debug.Log("Todavia no puedes usar la palanca. Falta el dialogo: " + dialogoNecesario.name);
            return;
        }

        StartCoroutine(ActivarPalanca());
    }

    private IEnumerator ActivarPalanca()
    {
        usada = true;

        yield return StartCoroutine(GirarPalanca());

        if (colliderSueloADesactivar != null)
        {
            colliderSueloADesactivar.enabled = false;
        }

        yield return new WaitForSeconds(esperaAntesDeFundido);

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

    private IEnumerator GirarPalanca()
    {
        if (palancaVisual == null)
        {
            yield break;
        }

        Quaternion rotacionInicial = palancaVisual.rotation;
        Quaternion rotacionFinal = rotacionInicial * Quaternion.Euler(0f, 0f, gradosGiro);

        float tiempo = 0f;

        while (tiempo < duracionGiro)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionGiro;

            palancaVisual.rotation = Quaternion.Lerp(rotacionInicial, rotacionFinal, t);

            yield return null;
        }

        palancaVisual.rotation = rotacionFinal;
    }
}
