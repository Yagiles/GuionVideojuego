using UnityEngine;
using TMPro;
using System.Collections;

public class ZonaBusquedaObjeto : MonoBehaviour
{
    [Header("Condicion para poder buscar")]
    public DialogoData dialogoNecesario;

    [Header("UI")]
    public TMP_Text avisoBuscar;
    public GameObject panelResultado;
    public TMP_Text textoResultado;

    [Header("Busqueda")]
    public bool contieneObjeto;
    public GameObject prefabObjetoAparecer;
    public Transform puntoAparicion;

    [Header("Tiempo mensaje")]
    public float tiempoMostrarResultado = 4f;

    private bool jugadorDentro = false;
    private bool objetoYaAparecido = false;
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

        if (contieneObjeto)
        {
            if (!objetoYaAparecido && prefabObjetoAparecer != null)
            {
                Instantiate(
                    prefabObjetoAparecer,
                    puntoAparicion != null
                        ? puntoAparicion.position
                        : transform.position,
                    Quaternion.identity
                );

                objetoYaAparecido = true;
            }

            if (panelResultado != null)
                panelResultado.SetActive(false);
        }
        else
        {
            MostrarResultado();
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

            if (panelResultado != null)
                panelResultado.SetActive(false);

            if (avisoBuscar != null)
                avisoBuscar.gameObject.SetActive(false);

            if (textoResultado != null)
                textoResultado.gameObject.SetActive(false);
        }
    }
}