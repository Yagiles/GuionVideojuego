using System.Collections;
using UnityEngine;

public class MoverRey : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 1.5f;
    public float tiempoMoverse = 5f;

    [Header("Personajes adicionales")]
    public Transform juglar;

    private NPCInteractuable npcInteractuable;
    private bool movimientoIniciado = false;

    private Animator animator;

    private void Awake()
    {
        npcInteractuable = GetComponent<NPCInteractuable>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (npcInteractuable == null) return;

        if (npcInteractuable.dialogoTerminado && !movimientoIniciado)
        {
            movimientoIniciado = true;
            StartCoroutine(Moverse());
        }
    }

    IEnumerator Moverse()
    {
        animator.SetBool("moviendose", true);

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoMoverse)
        {
            Vector3 movimiento = Vector3.right * velocidad * Time.deltaTime;

            transform.Translate(movimiento);

            if (juglar != null)
                juglar.Translate(movimiento);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        animator.SetBool("moviendose", false);
    }
}