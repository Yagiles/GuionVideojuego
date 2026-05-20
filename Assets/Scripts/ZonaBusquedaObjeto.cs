using UnityEngine;
using TMPro;
using System.Collections;

public class ZonaBusquedaObjeto : MonoBehaviour
{
    [Header("Condicion para poder buscar")]
    public DialogoData dialogoNecesario;

    [Header("Estado persistente")]
    public string idBusquedaCompletada = "busqueda_objeto_completada";

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
        if (BusquedaYaCompletada() || YaTieneObjetoBuscado())
        {
            OcultarUI();
            DesactivarSistemaBusqueda();
            gameObject.SetActive(false);
            return;
        }

        OcultarUI();
    }

    void Update()
    {
        if (BusquedaYaCompletada() || YaTieneObjetoBuscado())
        {
            OcultarUI();
            DesactivarSistemaBusqueda();
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

    bool BusquedaYaCompletada()
    {
        return EstadoDialogos.instancia != null &&
               !string.IsNullOrEmpty(idBusquedaCompletada) &&
               EstadoDialogos.instancia.HaHabladoCon(idBusquedaCompletada);
    }

    void MarcarBusquedaCompletada()
    {
        if (EstadoDialogos.instancia != null &&
            !string.IsNullOrEmpty(idBusquedaCompletada))
        {
            EstadoDialogos.instancia.MarcarComoHablado(idBusquedaCompletada);
        }
    }

    void Buscar()
    {
        if (BusquedaYaCompletada() || YaTieneObjetoBuscado())
        {
            OcultarUI();
            DesactivarSistemaBusqueda();
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

            MarcarBusquedaCompletada();
            OcultarUI();
            DesactivarSistemaBusqueda();
            gameObject.SetActive(false);
        }
        else
        {
            MostrarResultado();
        }
    }

    void DesactivarSistemaBusqueda()
    {
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
        if (BusquedaYaCompletada() || YaTieneObjetoBuscado())
        {
            OcultarUI();
            DesactivarSistemaBusqueda();
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