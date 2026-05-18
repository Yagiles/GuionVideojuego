using UnityEngine;

public class GuiaSigueJugadorVertical : MonoBehaviour
{
    private Transform jugador;
    private Rigidbody2D rb;

    [Header("Seguimiento")]
    public float velocidad = 6f;
    public Vector2 offsetRespectoJugador = new Vector2(2f, 2f);
    public float distanciaParada = 0.1f;

    [Header("Suavizado")]
    public float suavizado = 0.15f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");

        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
        else
        {
            Debug.LogError("No se encontro ningun objeto con tag Player");
        }
    }

    private void FixedUpdate()
    {
        if (jugador == null || rb == null) return;

        Vector2 posicionObjetivo = (Vector2)jugador.position + offsetRespectoJugador;
        Vector2 diferencia = posicionObjetivo - rb.position;

        if (diferencia.magnitude < distanciaParada)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 velocidadObjetivo = diferencia / suavizado;

        if (velocidadObjetivo.magnitude > velocidad)
        {
            velocidadObjetivo = velocidadObjetivo.normalized * velocidad;
        }

        rb.linearVelocity = velocidadObjetivo;
    }
}