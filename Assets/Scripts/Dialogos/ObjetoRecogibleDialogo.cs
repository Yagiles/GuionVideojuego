using UnityEngine;

public class ObjetoRecogibleDialogo : MonoBehaviour
{
    public ObjetoData objetoData;

    public bool recogido = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (InventarioManager.Instance != null && objetoData != null)
        {
            InventarioManager.Instance.AñadirObjeto(objetoData);
            InventarioUI.Instance.RefrescarUI();
        }

        recogido = true;
        Destroy(gameObject);
    }
}