using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class EventoBesoDialogo
{
    public DialogoData dialogo;
    public int despuesDeLinea;
    public BesoPersonajes beso;
    [HideInInspector] public bool ejecutado;
}

[System.Serializable]
public class EventoFlip
{
    public DialogoData dialogo;
    public int despuesDeLinea;
    public FlipPersonaje flip;
    [HideInInspector] public bool ejecutado;
}

[System.Serializable]
public class EventoMovimientoGuiaDialogoBosque
{
    public DialogoData dialogo;
    public int despuesDeLinea;
    public CinematicaGuiaMovimiento movimiento;

    [HideInInspector]
    public bool ejecutado;
}

[System.Serializable]
public class EventoActivarMisionDialogo
{
    public DialogoData dialogo;
    public int despuesDeLinea;
    public AccionMisionDialogo accion;
    [HideInInspector] public bool ejecutado;
}

public enum AccionMisionDialogo
{
    ActivarMisionActual,
    AvanzarYActivarSiguiente
}

public class DialogoManager : MonoBehaviour
{
    public GameObject panelDialogo;
    public TMP_Text textoNombre;
    public TMP_Text textoDialogo;
    public TMP_Text textoContinuar;

    private DialogoData dialogoActual;
    private int indiceLinea = 0;

    public bool dialogoActivo = false;
    private bool puedeAvanzar = false;

    public System.Action alTerminarDialogo;

    private Coroutine coroutineAutoDialogo;

    [Header("Eventos de beso")]
    public EventoBesoDialogo[] eventosBeso;

    [Header("Eventos de flip")]
    public EventoFlip[] eventosFlip;

    [Header("Eventos de movimiento")]
    public EventoMovimientoGuiaDialogoBosque[] eventosMovimiento;

    private bool ejecutandoEventoMovimiento = false;

    [Header("Eventos de activar mision")]
    public EventoActivarMisionDialogo[] eventosActivarMision;

    [Header("Efecto escritura")]
    public float velocidadEscritura = 0.03f;

    private bool escribiendo = false;
    private Coroutine coroutineEscritura;

    void Start()
    {
        panelDialogo.SetActive(false);

        if (textoContinuar != null)
        {
            textoContinuar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (dialogoActivo && dialogoActual != null && !dialogoActual.avanceAutomatico && Input.GetKeyDown(KeyCode.E))
        {
            if (ejecutandoEventoMovimiento) return;

            if (escribiendo)
            {
                CompletarTexto();
            }
            else if (puedeAvanzar)
            {
                SiguienteLinea();
            }
        }
    }

    public void IniciarDialogo(DialogoData dialogo)
    {
        if (dialogo == null) return;

        if (coroutineAutoDialogo != null)
        {
            StopCoroutine(coroutineAutoDialogo);
            coroutineAutoDialogo = null;
        }

        dialogoActual = dialogo;
        indiceLinea = 0;

        ReiniciarEventosBeso();
        ReiniciarEventosFlip();
        ReiniciarEventosMovimiento();
        ReiniciarEventosActivarMision();

        dialogoActivo = true;
        puedeAvanzar = false;

        panelDialogo.SetActive(true);
        MostrarLinea();

        if (textoContinuar != null)
        {
            textoContinuar.gameObject.SetActive(!dialogoActual.avanceAutomatico);

            if (!dialogoActual.avanceAutomatico)
            {
                textoContinuar.text = "Pulsa E para avanzar";
            }
        }

        if (dialogoActual.avanceAutomatico)
        {
            coroutineAutoDialogo = StartCoroutine(AutoAvanzarDialogo());
        }
        else
        {
            StartCoroutine(ActivarAvance());
        }
    }

    void MostrarLinea()
    {
        puedeAvanzar = false;

        textoNombre.text = dialogoActual.lineas[indiceLinea].nombrePersonaje;

        if (coroutineEscritura != null)
        {
            StopCoroutine(coroutineEscritura);
        }

        coroutineEscritura = StartCoroutine(
            EscribirTexto(dialogoActual.lineas[indiceLinea].texto)
        );
    }

    void SiguienteLinea()
    {
        if (ejecutandoEventoMovimiento) return;

        ComprobarEventosBeso();
        ComprobarEventosFlip();
       

        int lineaActual = indiceLinea + 1;

        EventoMovimientoGuiaDialogoBosque eventoMovimiento =
            BuscarEventoMovimiento(lineaActual);

        if (eventoMovimiento != null)
        {
            StartCoroutine(EjecutarMovimientoDespuesDeLinea(eventoMovimiento));
            return;
        }

        indiceLinea++;

        if (indiceLinea >= dialogoActual.lineas.Length)
        {
            TerminarDialogo();
        }
        else
        {
            MostrarLinea();
        }
    }

    void TerminarDialogo()
    {
        ComprobarEventosActivarMisionAlTerminar();

        if (coroutineAutoDialogo != null)
        {
            StopCoroutine(coroutineAutoDialogo);
            coroutineAutoDialogo = null;
        }

        panelDialogo.SetActive(false);

        if (textoContinuar != null)
        {
            textoContinuar.gameObject.SetActive(false);
        }

        dialogoActual = null;
        dialogoActivo = false;
        puedeAvanzar = false;

        alTerminarDialogo?.Invoke();
        alTerminarDialogo = null;
    }

    void ComprobarEventosBeso()
    {
        if (eventosBeso == null || dialogoActual == null) return;

        int lineaActual = indiceLinea + 1;

        for (int i = 0; i < eventosBeso.Length; i++)
        {
            EventoBesoDialogo evento = eventosBeso[i];

            if (evento == null) continue;

            if (
                !evento.ejecutado &&
                evento.dialogo == dialogoActual &&
                evento.despuesDeLinea == lineaActual &&
                evento.beso != null
            )
            {
                evento.beso.EmpezarBeso();
                evento.ejecutado = true;
            }
        }
    }

    void ReiniciarEventosBeso()
    {
        if (eventosBeso == null) return;

        for (int i = 0; i < eventosBeso.Length; i++)
        {
            eventosBeso[i].ejecutado = false;
        }
    }

    void ComprobarEventosFlip()
    {
        if (eventosFlip == null || dialogoActual == null) return;

        int lineaActual = indiceLinea + 1;

        for (int i = 0; i < eventosFlip.Length; i++)
        {
            EventoFlip evento = eventosFlip[i];

            if (evento == null) continue;

            if (
                !evento.ejecutado &&
                evento.dialogo == dialogoActual &&
                evento.despuesDeLinea == lineaActual &&
                evento.flip != null
            )
            {
                evento.flip.EjecutarFlip();
                evento.ejecutado = true;
            }
        }
    }

    void ReiniciarEventosFlip()
    {
        if (eventosFlip == null) return;

        for (int i = 0; i < eventosFlip.Length; i++)
        {
            eventosFlip[i].ejecutado = false;
        }
    }

    EventoMovimientoGuiaDialogoBosque BuscarEventoMovimiento(int numeroLinea)
    {
        if (eventosMovimiento == null || dialogoActual == null)
            return null;

        for (int i = 0; i < eventosMovimiento.Length; i++)
        {
            EventoMovimientoGuiaDialogoBosque evento = eventosMovimiento[i];

            if (
                evento != null &&
                !evento.ejecutado &&
                evento.dialogo == dialogoActual &&
                evento.despuesDeLinea == numeroLinea &&
                evento.movimiento != null
            )
            {
                return evento;
            }
        }

        return null;
    }

    IEnumerator EjecutarMovimientoDespuesDeLinea(EventoMovimientoGuiaDialogoBosque evento)
    {
        ejecutandoEventoMovimiento = true;
        evento.ejecutado = true;

        yield return StartCoroutine(evento.movimiento.MoverDerecha());

        ejecutandoEventoMovimiento = false;

        indiceLinea++;

        if (indiceLinea >= dialogoActual.lineas.Length)
        {
            TerminarDialogo();
        }
        else
        {
            MostrarLinea();
        }
    }

    void ReiniciarEventosMovimiento()
    {
        if (eventosMovimiento == null) return;

        for (int i = 0; i < eventosMovimiento.Length; i++)
        {
            eventosMovimiento[i].ejecutado = false;
        }
    }

    void ComprobarEventosActivarMision()
    {
        if (eventosActivarMision == null || dialogoActual == null) return;

        int lineaActual = indiceLinea + 1;

        for (int i = 0; i < eventosActivarMision.Length; i++)
        {
            EventoActivarMisionDialogo evento = eventosActivarMision[i];

            if (evento == null) continue;

            if (
                !evento.ejecutado &&
                evento.dialogo == dialogoActual &&
                evento.despuesDeLinea == lineaActual
            )
            {
                if (MisionManager.Instance != null)
                    MisionManager.Instance.AvanzarYActivarSiguienteMision();

                evento.ejecutado = true;
            }
        }
    }

    void ComprobarEventosActivarMisionAlTerminar()
    {
        if (eventosActivarMision == null || dialogoActual == null) return;

        for (int i = 0; i < eventosActivarMision.Length; i++)
        {
            EventoActivarMisionDialogo evento = eventosActivarMision[i];

            if (evento == null) continue;

            if (
                !evento.ejecutado &&
                evento.dialogo == dialogoActual
            )
            {
                if (MisionManager.Instance != null)
                {
                    if (evento.accion == AccionMisionDialogo.ActivarMisionActual)
                    {
                        MisionManager.Instance.ActivarMisionActual();
                    }
                    else if (evento.accion == AccionMisionDialogo.AvanzarYActivarSiguiente)
                    {
                        MisionManager.Instance.AvanzarYActivarSiguienteMision();
                    }
                }

                evento.ejecutado = true;
            }
        }
    }

    void ReiniciarEventosActivarMision()
    {
        if (eventosActivarMision == null) return;

        for (int i = 0; i < eventosActivarMision.Length; i++)
        {
            eventosActivarMision[i].ejecutado = false;
        }
    }

    private IEnumerator ActivarAvance()
    {
        yield return null;
        puedeAvanzar = true;
    }

    private IEnumerator AutoAvanzarDialogo()
    {
        while (dialogoActivo)
        {
            while (escribiendo)
            {
                yield return null;
            }

            yield return new WaitForSeconds(dialogoActual.duracionLinea);

            if (dialogoActivo)
            {
                SiguienteLinea();
            }
        }
    }

    IEnumerator EscribirTexto(string texto)
    {
        escribiendo = true;
        textoDialogo.text = "";

        bool dentroEtiqueta = false;

        foreach (char letra in texto)
        {
            if (letra == '<')
            {
                dentroEtiqueta = true;
            }

            textoDialogo.text += letra;

            if (letra == '>')
            {
                dentroEtiqueta = false;
                continue;
            }

            if (!dentroEtiqueta)
            {
                yield return new WaitForSeconds(velocidadEscritura);
            }
        }

        escribiendo = false;
        puedeAvanzar = true;
    }

    void CompletarTexto()
    {
        if (coroutineEscritura != null)
        {
            StopCoroutine(coroutineEscritura);
        }

        textoDialogo.text = dialogoActual.lineas[indiceLinea].texto;

        escribiendo = false;
        puedeAvanzar = true;
    }
}