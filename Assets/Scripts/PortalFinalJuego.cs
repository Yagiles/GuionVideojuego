using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalFinalJuego : MonoBehaviour
{
    [Header("UI opciones")]
    public GameObject panelQuedarse;
    public GameObject panelIrse;

    [Header("Dialogo si te quedas")]
    public DialogoManager dialogoManager;
    public DialogoData dialogoQuedarse;

    [Header("Pantalla negra final")]
    public CanvasGroup pantallaNegra;
    public float duracionFundido = 1f;

    [Header("Final")]
    public bool cargarEscenaFinal = false;
    public string nombreEscenaFinal;

    private bool jugadorDentro = false;
    private bool finalActivado = false;

    private void Start()
    {
        if (panelQuedarse != null)
        {
            panelQuedarse.SetActive(false);
        }

        if (panelIrse != null)
        {
            panelIrse.SetActive(false);
        }

        if (pantallaNegra != null)
        {
            pantallaNegra.alpha = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        jugadorDentro = true;

        MostrarOpciones();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        jugadorDentro = false;

        if (panelQuedarse != null)
        {
            panelQuedarse.SetActive(false);
        }

        if (panelIrse != null)
        {
            panelIrse.SetActive(false);
        }
    }

    private void MostrarOpciones()
    {
        if (finalActivado) return;

        if (panelQuedarse != null)
        {
            panelQuedarse.SetActive(true);
        }

        if (panelIrse != null)
        {
            panelIrse.SetActive(true);
        }
    }

    public void ElegirQuedarse()
    {
        if (finalActivado) return;

        finalActivado = true;

        if (panelQuedarse != null)
        {
            panelQuedarse.SetActive(false);
        }

        if (panelIrse != null)
        {
            panelIrse.SetActive(false);
        }

        if (dialogoManager != null && dialogoQuedarse != null)
        {
            dialogoManager.alTerminarDialogo = FinalizarJuego;
            dialogoManager.IniciarDialogo(dialogoQuedarse);
        }
        else
        {
            FinalizarJuego();
        }
    }

    public void ElegirIrse()
    {
        if (finalActivado) return;

        finalActivado = true;

        if (panelQuedarse != null)
        {
            panelQuedarse.SetActive(false);
        }

        if (panelIrse != null)
        {
            panelIrse.SetActive(false);
        }

        FinalizarJuego();
    }

    private void FinalizarJuego()
    {
        StartCoroutine(FundidoFinal());
    }

    private IEnumerator FundidoFinal()
    {
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

        if (cargarEscenaFinal)
        {
            SceneManager.LoadScene(nombreEscenaFinal);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}