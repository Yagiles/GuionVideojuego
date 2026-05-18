using UnityEngine;

public class ObjetoRecogibleDialogo : MonoBehaviour
{
    [HideInInspector] public DialogoManager dialogoManager;
    public bool recogido = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        recogido = true;
        Destroy(gameObject);
    }
}