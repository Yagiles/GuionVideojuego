using Unity.VisualScripting;
using UnityEngine;

public class ScriptParaSolucionarCosasQueEsperoPorfavorQueSeanSolucionables : MonoBehaviour
{
    public bool solucionado = false;
    public DialogoManager dialogoManager;

    private void Start()
    {
        if (dialogoManager == null)
        {
            dialogoManager = FindAnyObjectByType<DialogoManager>();
            if (dialogoManager == null)
            {
                Debug.LogError("No se encontró un DialogoManager en la escena.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!solucionado)
            {
                Debug.Log("Solucionando cosas que espero sean solucionables");
                dialogoManager.dialogoActivo = false;
                solucionado = true;
            }
        }
    }
}
