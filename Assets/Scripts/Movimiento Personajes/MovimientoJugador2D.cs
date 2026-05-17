using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador2D : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 8f;
    public int saltosMaximos = 2;

    private float movimientoX;
    private Rigidbody2D rb;
    private bool enSuelo;
    private int saltosRestantes;

    public Transform detectorSuelo;
    public float radioSuelo = 0.2f;
    public LayerMask capaSuelo;

    private Animator animator; //animacion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        saltosRestantes = saltosMaximos;
        animator = GetComponent<Animator>(); //animacion
    }

    void Update()
    {
        // Detectar si esta en el suelo
        enSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioSuelo, capaSuelo);

        // Reiniciar saltos al tocar el suelo
        if (enSuelo)
        {
            saltosRestantes = saltosMaximos;
        }

        // Animación
        animator.SetFloat("velocidad", Mathf.Abs(movimientoX));
    }

    void FixedUpdate()
    {
        // Movimiento horizontal
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
    }

    public void OnMove(InputValue value)
    {
        movimientoX = value.Get<Vector2>().x;
        Vector3 escala = transform.localScale;

        if (movimientoX > 0)
        {
            escala.x = Mathf.Abs(escala.x);
        }
        else if (movimientoX < 0)
        {
            escala.x = -Mathf.Abs(escala.x);
        }

        transform.localScale = escala;
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        // Solo salta si quedan saltos disponibles
        if (saltosRestantes > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

            saltosRestantes--;
        }
    }
}
