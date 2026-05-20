using System.Collections;
using UnityEngine;

public class MoverReyAlActivarTrigger : CinematicaGuiaMovimiento
{
    [Header("Rey")]
    public GameObject rey;
    public Transform puntoA;
    public Transform puntoB;

    [Header("Personaje adicional opcional")]
    public Transform juglar;

    private bool iniciado = false;
    private Animator animatorRey;

    public override IEnumerator MoverDerecha()
    {
        if (iniciado)
            yield break;

        iniciado = true;

        if (rey == null || puntoA == null || puntoB == null)
        {
            Debug.LogWarning("Faltan referencias en MoverReyAlActivarTrigger");
            yield break;
        }

        rey.SetActive(true);
        rey.transform.position = puntoA.position;

        animatorRey = rey.GetComponent<Animator>();

        if (animatorRey != null)
            animatorRey.SetBool("moviendose", true);

        while (Vector3.Distance(rey.transform.position, puntoB.position) > 0.05f)
        {
            Vector3 posicionAnterior = rey.transform.position;

            rey.transform.position = Vector3.MoveTowards(
                rey.transform.position,
                puntoB.position,
                velocidad * Time.deltaTime
            );

            Vector3 desplazamiento = rey.transform.position - posicionAnterior;

            if (juglar != null)
                juglar.position += desplazamiento;

            yield return null;
        }

        rey.transform.position = puntoB.position;

        if (animatorRey != null)
            animatorRey.SetBool("moviendose", false);
    }
}