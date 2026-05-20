using UnityEngine;

public class DialogoMapacheObjeto : MonoBehaviour
{
    [Header("Condicion previa")]
    public DialogoData dialogoPrimeroNecesario;

    [Header("Objeto necesario para hablar")]
    public ObjetoData objetoNecesarioParaHablar;

    [Header("Dialogo")]
    public DialogoData dialogoMapacheObjeto;
    public DialogoManager dialogoManager;

    [Header("Movimiento a bloquear")]
    public MonoBehaviour scriptMovimientoJugador;
    public Rigidbody2D rbJugador;

    [Header("Bloqueador lateral")]
    public GameObject bloqueadorMovimientoLateral;

    private bool jugadorCerca = false;
    private bool dialogoEnCurso = false;
    private bool esperandoSoltarE = false;

    private void Start()
    {
        if (bloqueadorMovimientoLateral != null)
            bloqueadorMovimientoLateral.SetActive(false);
    }

    private void Update()
    {
        if (esperandoSoltarE)
        {
            if (!Input.GetKey(KeyCode.E))
                esperandoSoltarE = false;

            return;
        }

        if (jugadorCerca && !dialogoEnCurso && Input.GetKeyDown(KeyCode.E))
        {
            IntentarHablar();
        }
    }

    void IntentarHablar()
    {
        if (dialogoManager == null) return;
        if (dialogoMapacheObjeto == null) return;

        if (EstadoDialogos.instancia == null) return;

        if (dialogoPrimeroNecesario != null &&
            !EstadoDialogos.instancia.HaHabladoCon(dialogoPrimeroNecesario.name))
        {
            Debug.Log("Falta el primer dialogo: " + dialogoPrimeroNecesario.name);
            return;
        }

        if (InventarioManager.Instance == null) return;

        if (objetoNecesarioParaHablar == null)
        {
            Debug.LogWarning("Falta asignar el objeto necesario para hablar.");
            return;
        }

        if (!InventarioManager.Instance.TieneObjeto(objetoNecesarioParaHablar))
        {
            Debug.Log("No tienes el objeto necesario: " + objetoNecesarioParaHablar.nombreObjeto);
            return;
        }

        dialogoEnCurso = true;

        if (bloqueadorMovimientoLateral != null)
            bloqueadorMovimientoLateral.SetActive(true);

        BloquearMovimientoJugador();

        dialogoManager.alTerminarDialogo = TerminarDialogoMapache;
        dialogoManager.IniciarDialogo(dialogoMapacheObjeto);
    }

    void TerminarDialogoMapache()
    {
        if (bloqueadorMovimientoLateral != null)
            bloqueadorMovimientoLateral.SetActive(false);

        DesbloquearMovimientoJugador();

        dialogoEnCurso = false;
        esperandoSoltarE = true;

        if (EstadoDialogos.instancia != null && dialogoMapacheObjeto != null)
        {
            EstadoDialogos.instancia.MarcarComoHablado(dialogoMapacheObjeto.name);
        }
    }

    void BloquearMovimientoJugador()
    {
        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = false;

        if (rbJugador != null)
        {
            rbJugador.linearVelocity = Vector2.zero;
            rbJugador.angularVelocity = 0f;
        }
    }

    void DesbloquearMovimientoJugador()
    {
        if (rbJugador != null)
        {
            rbJugador.linearVelocity = Vector2.zero;
            rbJugador.angularVelocity = 0f;
        }

        if (scriptMovimientoJugador != null)
            scriptMovimientoJugador.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            esperandoSoltarE = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
            esperandoSoltarE = false;
        }
    }
}