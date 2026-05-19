using System.Collections;
using UnityEngine;

public class TriggerCinematicaDialogo : MonoBehaviour
{
    public DialogoData dialogo;
    public DialogoManager dialogoManager;

    [Header("Scripts a bloquear")]
    public MonoBehaviour scriptMovimientoJugador;
    public MonoBehaviour scriptMovimientoGuia;

    [Header("Rigidbodies a parar")]
    public Rigidbody2D rbJugador;
    public Rigidbody2D rbGuia;

    [Header("Pantalla negra")]
    public CanvasGroup pantallaNegra;
    public float duracionFundido = 1f;

    [Header("Personajes principales")]
    public Transform jugador;
    public Transform destinoJugador;

    public Transform guia;

    [Tooltip("Posicion exacta donde se coloca el guia al EMPEZAR la cinematica")]
    public Transform posicionInicialGuiaCinematica;

    [Tooltip("Posicion del guia DESPUES de la pantalla negra")]
    public Transform destinoGuia;

    [Header("Otros personajes")]
    public Transform[] personajes;
    public Transform[] destinosPersonajes;

    [Header("Opciones")]
    public bool reactivarMovimientoGuiaAlFinal = true;

    [Header("Solo una vez")]
    public bool destruirTrasActivarse = true;

    [Header("Activar y desactivar al terminar")]
    public GameObject[] objetosActivarAlFinal;
    public GameObject[] objetosDesactivarAlFinal;

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activado) return;

        if (!collision.CompareTag("Player")) return;

        if (EstadoDialogos.instancia != null && dialogo != null)
        {
            if (EstadoDialogos.instancia.HaHabladoCon(dialogo.name))
            {
                return;
            }
        }

        activado = true;

        BloquearMovimiento();

        if (guia != null && posicionInicialGuiaCinematica != null)
        {
            guia.position = posicionInicialGuiaCinematica.position;
        }

        if (rbGuia != null)
        {
            rbGuia.linearVelocity = Vector2.zero;
        }

        dialogoManager.alTerminarDialogo = TerminarCinematica;
        dialogoManager.IniciarDialogo(dialogo);
    }

    void BloquearMovimiento()
    {
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = false;

        if (scriptMovimientoGuia != null)
            scriptMovimientoGuia.enabled = false;

        if (rbJugador != null)
            rbJugador.linearVelocity = Vector2.zero;

        if (rbGuia != null)
            rbGuia.linearVelocity = Vector2.zero;
    }

    void TerminarCinematica()
    {
        StartCoroutine(FinalizarConFundido());
    }

    IEnumerator FinalizarConFundido()
    {
        // Fundido a negro
        if (pantallaNegra != null)
        {
            pantallaNegra.gameObject.SetActive(true);

            float tiempo = 0f;

            while (tiempo < duracionFundido)
            {
                tiempo += Time.deltaTime;
                pantallaNegra.alpha = tiempo / duracionFundido;
                yield return null;
            }

            pantallaNegra.alpha = 1f;
        }

        // ACTIVAR Y DESACTIVAR COSAS MIENTRAS LA PANTALLA ESTA NEGRA
        if (objetosDesactivarAlFinal != null)
        {
            for (int i = 0; i < objetosDesactivarAlFinal.Length; i++)
            {
                if (objetosDesactivarAlFinal[i] != null)
                {
                    objetosDesactivarAlFinal[i].SetActive(false);
                }
            }
        }

        if (objetosActivarAlFinal != null)
        {
            for (int i = 0; i < objetosActivarAlFinal.Length; i++)
            {
                if (objetosActivarAlFinal[i] != null)
                {
                    objetosActivarAlFinal[i].SetActive(true);
                }
            }
        }

        // Mover jugador y guia
        if (jugador != null && destinoJugador != null)
        {
            jugador.position = destinoJugador.position;

            if (rbJugador != null)
            {
                rbJugador.linearVelocity = Vector2.zero;
                rbJugador.angularVelocity = 0f;
            }
        }

        if (guia != null && destinoGuia != null)
        {
            guia.position = destinoGuia.position;
        }

        // Mover otros personajes
        int cantidad = Mathf.Min(personajes.Length, destinosPersonajes.Length);

        for (int i = 0; i < cantidad; i++)
        {
            if (personajes[i] != null && destinosPersonajes[i] != null)
            {
                personajes[i].position = destinosPersonajes[i].position;
            }
        }

        // Parar velocidades despues de moverlos
        if (rbJugador != null)
        {
            rbJugador.linearVelocity = Vector2.zero;
            rbJugador.angularVelocity = 0f;
        }

        if (rbGuia != null)
        {
            rbGuia.linearVelocity = Vector2.zero;
            rbGuia.angularVelocity = 0f;
        }

        // Fundido desde negro
        if (pantallaNegra != null)
        {
            float tiempo = 0f;

            while (tiempo < duracionFundido)
            {
                tiempo += Time.deltaTime;
                pantallaNegra.alpha = 1f - (tiempo / duracionFundido);
                yield return null;
            }

            pantallaNegra.alpha = 0f;
        }

        // Desbloquear movimiento jugador
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = true;

        // Reactivar o no el guia
        if (scriptMovimientoGuia != null && reactivarMovimientoGuiaAlFinal)
            scriptMovimientoGuia.enabled = true;

        if (destruirTrasActivarse)
            Destroy(gameObject);
    }
}