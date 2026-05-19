using UnityEngine;

public class BloqueoTemporalDialogo : MonoBehaviour
{
    [Header("Collider que bloquea el paso")]
    public Collider2D colliderBloqueo;

    private void Start()
    {
        if (colliderBloqueo != null)
        {
            colliderBloqueo.enabled = false;
        }
    }

    public void ActivarBloqueo()
    {
        if (colliderBloqueo != null)
        {
            colliderBloqueo.enabled = true;
        }
    }

    public void DesactivarBloqueo()
    {
        if (colliderBloqueo != null)
        {
            colliderBloqueo.enabled = false;
        }
    }
}