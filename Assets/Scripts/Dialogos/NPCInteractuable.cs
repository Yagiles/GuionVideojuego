using UnityEngine;

public class NPCInteractuable : MonoBehaviour
{
    [Header("Identificador de este personaje")]
    public string idPersonaje;

    [Header("Dialogo")]
    public DialogoData dialogo;
    public DialogoManager dialogoManager;

    [Header("Condiciones para poder hablar")]
    public string[] idsNecesariosParaHablar;

    [Header("Condicion por objeto")]
    public bool requiereObjeto = false;
    public ObjetoData objetoNecesario;

    [Header("Movimiento a bloquear")]
    public MonoBehaviour scriptMovimientoJugador;
    public Rigidbody2D rbJugador;

    [Header("Bloqueo temporal")]
    public Collider2D colliderBloqueoTemporal;

    [Header("Misiones")]
    public bool completaMision = false;

    private bool jugadorCerca = false;
    private bool dialogoEnCurso = false;
    private bool esperandoSoltarE = false;
    public bool dialogoTerminado { get; private set; } = false;

    private void Start()
    {
        Debug.Log("[NPCInteractuable] Start en NPC: " + gameObject.name);

        if (colliderBloqueoTemporal != null)
        {
            colliderBloqueoTemporal.enabled = false;
            Debug.Log("[NPCInteractuable] Collider bloqueo temporal desactivado: " + colliderBloqueoTemporal.name);
        }
    }

    private void Update()
    {
        if (esperandoSoltarE)
        {
            if (!Input.GetKey(KeyCode.E))
            {
                esperandoSoltarE = false;
                Debug.Log("[NPCInteractuable] Ya se solto la E. Se permite interactuar otra vez.");
            }

            return;
        }

        if (jugadorCerca && !dialogoEnCurso && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[NPCInteractuable] Se ha pulsado E cerca del NPC: " + gameObject.name);
            IntentarHablar();
        }
    }

    void IntentarHablar()
    {
        Debug.Log("[NPCInteractuable] IntentarHablar en: " + gameObject.name);

        if (dialogoManager == null)
        {
            Debug.LogWarning("[NPCInteractuable] No hay DialogoManager asignado en: " + gameObject.name);
            return;
        }

        if (dialogo == null)
        {
            Debug.LogWarning("[NPCInteractuable] No hay DialogoData asignado en: " + gameObject.name);
            return;
        }

        if (!CumpleCondiciones())
        {
            Debug.Log("[NPCInteractuable] No cumple condiciones para hablar con: " + gameObject.name);
            return;
        }

        Debug.Log("[NPCInteractuable] Cumple condiciones. Iniciando dialogo: " + dialogo.name);

        dialogoEnCurso = true;

        if (colliderBloqueoTemporal != null)
        {
            colliderBloqueoTemporal.enabled = true;
            Debug.Log("[NPCInteractuable] Collider bloqueo temporal activado: " + colliderBloqueoTemporal.name);
        }

        BloquearMovimientoJugador();

        dialogoManager.alTerminarDialogo = TerminarDialogoNPC;
        dialogoManager.IniciarDialogo(dialogo);
    }

    bool CumpleCondiciones()
    {
        Debug.Log("[NPCInteractuable] Comprobando condiciones de: " + gameObject.name);

        if (idsNecesariosParaHablar != null && idsNecesariosParaHablar.Length > 0)
        {
            Debug.Log("[NPCInteractuable] Tiene " + idsNecesariosParaHablar.Length + " ids necesarios para hablar.");

            for (int i = 0; i < idsNecesariosParaHablar.Length; i++)
            {
                Debug.Log("[NPCInteractuable] Comprobando id necesario: " + idsNecesariosParaHablar[i]);

                if (EstadoDialogos.instancia == null)
                {
                    Debug.LogWarning("[NPCInteractuable] No existe EstadoDialogos");
                    return false;
                }

                if (!EstadoDialogos.instancia.HaHabladoCon(idsNecesariosParaHablar[i]))
                {
                    Debug.Log("[NPCInteractuable] Falta haber hablado con: " + idsNecesariosParaHablar[i]);
                    return false;
                }

                Debug.Log("[NPCInteractuable] Id cumplido: " + idsNecesariosParaHablar[i]);
            }
        }
        else
        {
            Debug.Log("[NPCInteractuable] No tiene ids necesarios para hablar.");
        }

        if (requiereObjeto)
        {
            Debug.Log("[NPCInteractuable] Este NPC requiere objeto.");

            if (InventarioManager.Instance == null)
            {
                Debug.LogWarning("[NPCInteractuable] No existe InventarioManager");
                return false;
            }

            if (objetoNecesario == null)
            {
                Debug.LogWarning("[NPCInteractuable] Este NPC requiere un objeto, pero no se ha asignado Objeto Necesario");
                return false;
            }

            Debug.Log("[NPCInteractuable] Objeto necesario: " + objetoNecesario.nombreObjeto);

            if (!InventarioManager.Instance.TieneObjeto(objetoNecesario))
            {
                Debug.Log("[NPCInteractuable] No tienes el objeto necesario: " + objetoNecesario.nombreObjeto);
                return false;
            }

            Debug.Log("[NPCInteractuable] Si tienes el objeto necesario: " + objetoNecesario.nombreObjeto);
        }
        else
        {
            Debug.Log("[NPCInteractuable] Este NPC no requiere objeto.");
        }

        return true;
    }

    void TerminarDialogoNPC()
    {
        Debug.Log("[NPCInteractuable] TerminarDialogoNPC en: " + gameObject.name);

        if (EstadoDialogos.instancia != null)
        {
            EstadoDialogos.instancia.MarcarComoHablado(idPersonaje);
            Debug.Log("[NPCInteractuable] Marcado como hablado: " + idPersonaje);
        }
        else
        {
            Debug.LogWarning("[NPCInteractuable] No existe EstadoDialogos al terminar dialogo.");
        }

        if (colliderBloqueoTemporal != null)
        {
            colliderBloqueoTemporal.enabled = false;
            Debug.Log("[NPCInteractuable] Collider bloqueo temporal desactivado: " + colliderBloqueoTemporal.name);
        }

        DesbloquearMovimientoJugador();

        dialogoEnCurso = false;
        esperandoSoltarE = true;
        dialogoTerminado = true;

        if (completaMision && MisionManager.Instance != null)
        {
            MisionManager.Instance.NotificarDialogoTerminado(idPersonaje);
            Debug.Log("[NPCInteractuable] Mision notificada como completada por: " + idPersonaje);
        }
    }

    void BloquearMovimientoJugador()
    {
        Debug.Log("[NPCInteractuable] Bloqueando movimiento jugador.");

        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.enabled = false;
            Debug.Log("[NPCInteractuable] Script movimiento jugador desactivado.");
        }
        else
        {
            Debug.LogWarning("[NPCInteractuable] No hay scriptMovimientoJugador asignado.");
        }

        if (rbJugador != null)
        {
            rbJugador.linearVelocity = Vector2.zero;
            rbJugador.angularVelocity = 0f;
            Debug.Log("[NPCInteractuable] Rigidbody jugador parado.");
        }
        else
        {
            Debug.LogWarning("[NPCInteractuable] No hay rbJugador asignado.");
        }
    }

    void DesbloquearMovimientoJugador()
    {
        Debug.Log("[NPCInteractuable] Desbloqueando movimiento jugador.");

        if (rbJugador != null)
        {
            rbJugador.linearVelocity = Vector2.zero;
            rbJugador.angularVelocity = 0f;
        }

        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.enabled = true;
            Debug.Log("[NPCInteractuable] Script movimiento jugador activado.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[NPCInteractuable] Entro en trigger: " + collision.name + " con tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            esperandoSoltarE = false;
            Debug.Log("[NPCInteractuable] Jugador cerca de: " + gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("[NPCInteractuable] Salio del trigger: " + collision.name + " con tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
            esperandoSoltarE = false;
            Debug.Log("[NPCInteractuable] Jugador lejos de: " + gameObject.name);
        }
    }
}