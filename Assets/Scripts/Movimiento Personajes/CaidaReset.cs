using UnityEngine;

public class CaidaReset : MonoBehaviour
{
    [Header("Jugador")]
    public Transform jugador;
    public Rigidbody2D rbJugador;

    [Header("Puntos de respawn")]
    public Transform spawnInicio;
    public Transform spawnFinal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform spawnElegido = ElegirSpawnMasCercano();

            if (spawnElegido != null)
                jugador.position = spawnElegido.position;

            if (rbJugador != null)
            {
                rbJugador.linearVelocity = Vector2.zero;
                rbJugador.angularVelocity = 0f;
            }
        }
    }

    Transform ElegirSpawnMasCercano()
    {
        if (spawnInicio == null)
            return spawnFinal;

        if (spawnFinal == null)
            return spawnInicio;

        float distanciaAInicio = Vector2.Distance(jugador.position, spawnInicio.position);
        float distanciaAFinal = Vector2.Distance(jugador.position, spawnFinal.position);

        if (distanciaAInicio <= distanciaAFinal)
            return spawnInicio;
        else
            return spawnFinal;
    }
}