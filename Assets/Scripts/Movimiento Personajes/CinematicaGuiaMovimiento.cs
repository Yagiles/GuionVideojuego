using System.Collections;
using UnityEngine;

public class CinematicaGuiaMovimiento : MonoBehaviour
{
    public Transform personaje;

    [Header("Orientacion antes de moverse")]
    public bool mirarDerecha = true;
    public float pausaDespuesDelFlip = 0.3f;

    [Header("Movimiento")]
    public float distancia = 2f;
    public float velocidad = 1.5f;

    public virtual IEnumerator MoverDerecha()
    {
        GirarPersonaje();

        yield return new WaitForSeconds(pausaDespuesDelFlip);

        Vector3 inicio = personaje.position;
        Vector3 destino = inicio + Vector3.right * distancia;

        while (Vector3.Distance(personaje.position, destino) > 0.02f)
        {
            personaje.position = Vector3.MoveTowards(
                personaje.position,
                destino,
                velocidad * Time.deltaTime
            );

            yield return null;
        }

        personaje.position = destino;
    }

    void GirarPersonaje()
    {
        if (personaje == null) return;

        Vector3 escala = personaje.localScale;

        if (mirarDerecha)
            escala.x = Mathf.Abs(escala.x);
        else
            escala.x = -Mathf.Abs(escala.x);

        personaje.localScale = escala;
    }
}