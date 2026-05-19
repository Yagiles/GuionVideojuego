using UnityEngine;
using TMPro;
using System.Collections;

public class ZonaBusquedaNina : MonoBehaviour
{
    [Header("Condicion para poder buscar")]
    public DialogoData dialogoNecesario;

    [Header("UI")]
    public TMP_Text avisoBuscar;
    public GameObject panelResultado;
    public TMP_Text textoResultado;

    [Header("Busqueda")]
    public bool contieneNina;
    public GameObject ninaAActivar;
    public Transform puntoAparicionNina;

    [Header("Dialogo al encontrarla")]
    public DialogoManager dialogoManager;
    public DialogoData dialogoAlEncontrarla;

    [Header("Objetos a activar/desactivar al encontrarla")]
    public GameObject[] objetosActivarAlEncontrar;
    public GameObject[] objetosDesactivarAlEncontrar;

    [Header("Tiempo mensaje")]
    public float tiempoMostrarResultado = 4f;

    private bool jugadorDentro = false;
    private bool ninaEncontrada = false;
    private Coroutine coroutineOcultarResultado;

    void Start()
    {
        if (panelResultado != null)
            panelResultado.SetActive(false);

        if (avisoBuscar != null)
            avisoBuscar.gameObject.SetActive(false);

        if (textoResultado != null)
            textoResultado.gameObject.SetActive(false);
    }

    void Update()
    {
        if (ninaEncontrada) return;

        if (jugadorDentro && PuedeBuscar() && Input.GetKeyDown(KeyCode.B))
        {
            Buscar();
        }
    }

    bool PuedeBuscar()
    {
        if (dialogoNecesario == null)
            return true;

        return EstadoDialogos.instancia != null &&
               EstadoDialogos.instancia.HaHabladoCon(dialogoNecesario.name);
    }

    void Buscar()
    {
        if (avisoBuscar != null)
            avisoBuscar.gameObject.SetActive(false);

        if (contieneNina)
        {
            EncontrarNina();
        }
        else
        {
            MostrarResultado();
        }
    }

    void EncontrarNina()
    {
        ninaEncontrada = true;

        OcultarUI();

        if (ninaAActivar != null)
        {
            if (puntoAparicionNina != null)
            {
                ninaAActivar.transform.position = puntoAparicionNina.position;
            }

            ninaAActivar.SetActive(true);
        }

        for (int i = 0; i < objetosDesactivarAlEncontrar.Length; i++)
        {
            if (objetosDesactivarAlEncontrar[i] != null)
                objetosDesactivarAlEncontrar[i].SetActive(false);
        }

        for (int i = 0; i < objetosActivarAlEncontrar.Length; i++)
        {
            if (objetosActivarAlEncontrar[i] != null)
                objetosActivarAlEncontrar[i].SetActive(true);
        }

        if (dialogoManager != null && dialogoAlEncontrarla != null)
        {
            dialogoManager.IniciarDialogo(dialogoAlEncontrarla);
        }

        gameObject.SetActive(false);
    }

    void MostrarResultado()
    {
        if (panelResultado != null)
            panelResultado.SetActive(true);

        if (avisoBuscar != null)
            avisoBuscar.gameObject.SetActive(false);

        if (textoResultado != null)
            textoResultado.gameObject.SetActive(true);

        if (coroutineOcultarResultado != null)
            StopCoroutine(coroutineOcultarResultado);

        coroutineOcultarResultado = StartCoroutine(OcultarResultado());
    }

    IEnumerator OcultarResultado()
    {
        yield return new WaitForSeconds(tiempoMostrarResultado);

        if (panelResultado != null)
            panelResultado.SetActive(false);

        coroutineOcultarResultado = null;
    }

    void OcultarUI()
    {
        if (panelResultado != null)
            panelResultado.SetActive(false);

        if (avisoBuscar != null)
            avisoBuscar.gameObject.SetActive(false);

        if (textoResultado != null)
            textoResultado.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ninaEncontrada) return;

        if (collision.CompareTag("Player") && PuedeBuscar())
        {
            jugadorDentro = true;

            if (panelResultado != null)
                panelResultado.SetActive(true);

            if (textoResultado != null)
                textoResultado.gameObject.SetActive(false);

            if (avisoBuscar != null)
                avisoBuscar.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = false;
            OcultarUI();
        }
    }
}