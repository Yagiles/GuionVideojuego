using UnityEngine;

public class OrientacionInicialGuia : MonoBehaviour
{
    [Header("Direccion inicial de esta escena")]
    public int direccionIda = 1; // 1 derecha, -1 izquierda

    [Header("Configuracion visual")]
    public bool miraDerechaPorDefecto = true;

    [Header("Opcional")]
    public GuiaMovimiento guiaMovimiento;

    private void Start()
    {
        AplicarOrientacionInicial();
    }

    public void AplicarOrientacionInicial()
    {
        int direccionFinal = direccionIda;

        if (DatosCambioEscena.modoVuelta)
        {
            direccionFinal *= -1;
        }

        MirarHacia(direccionFinal);

        if (guiaMovimiento != null)
        {
            guiaMovimiento.modoVuelta = DatosCambioEscena.modoVuelta;
        }
    }

    private void MirarHacia(int direccion)
    {
        Vector3 escala = transform.localScale;

        float escalaXAbsoluta = Mathf.Abs(escala.x);

        bool debeMirarDerecha = direccion > 0;

        if (miraDerechaPorDefecto)
        {
            escala.x = debeMirarDerecha ? escalaXAbsoluta : -escalaXAbsoluta;
        }
        else
        {
            escala.x = debeMirarDerecha ? -escalaXAbsoluta : escalaXAbsoluta;
        }

        transform.localScale = escala;
    }
}