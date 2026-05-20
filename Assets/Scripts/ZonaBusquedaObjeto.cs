using UnityEngine;
using TMPro;
using System.Collections;

public class ZonaBusquedaObjeto : MonoBehaviour
{
    [Header("Condicion para poder buscar")]
    public DialogoData dialogoNecesario;

    [Header("Objeto que se busca")]
    public ObjetoData objetoBuscado;

    [Header("UI")]
    public TMP_Text avisoBuscar;
    public GameObject panelResultado;
    public TMP_Text textoResultado;

    [Header("Busqueda")]
    public bool contieneObjeto;
    public GameObject prefabObjetoAparecer;
    public Transform puntoAparicion;

    [Header("Sistema de busqueda")]
    public GameObject sistemaBusquedaObjeto;

    [Header("Tiempo mensaje")]
    public float tiempoMostrarResultado = 4f;

    private bool jugadorDentro = false;
    private bool objetoYaAparecido = false;
    private Coroutine coroutineOcultarResultado;

    void Start()
    {
        if (YaTieneObjetoBuscado())
        {
            if (sistemaBusquedaObjeto != null)
                sistemaBusquedaObjeto.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

        if (panelResultado != null)
            panelResultado.SetActive(false);

        if (avisoBuscar != null)
            avisoBuscar.gameObject.SetActive(false);

        if (textoResultado != null)
            textoResultado.gameObject.SetActive(false);
    }

    void Update()
    {
        if (YaTieneObjetoBuscado())
        {
            OcultarUI();

            if (sistemaBusquedaObjeto != null)
                sistemaBusquedaObjeto.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

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

    bool YaTieneObjetoBuscado()
    {
        return objetoBuscado != null &&
               InventarioManager.Instance != null &&
               InventarioManager.Instance.TieneObjeto(objetoBuscado);
    }

    void Buscar()
    {
        if (YaTieneObjetoBuscado())
        {
            OcultarUI();

            if (sistemaBusquedaObjeto != null)
                sistemaBusquedaObjeto.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

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

            OcultarUI();

            if (sistemaBusquedaObjeto != null)
            {
                sistemaBusquedaObjeto.SetActive(false);
            }
            else
            {
                GameObject busqueda = GameObject.Find("BusquedaObjeto");

                if (busqueda != null)
                {
                    busqueda.SetActive(false);
                }
            }

            gameObject.SetActive(false);
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
        if (YaTieneObjetoBuscado())
        {
            OcultarUI();

            if (sistemaBusquedaObjeto != null)
                sistemaBusquedaObjeto.SetActive(false);

            gameObject.SetActive(false);
            return;
        }

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