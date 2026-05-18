using UnityEngine;

public class DialogoTrigger : MonoBehaviour
{
    [Header("Diálogos")]
    public DialogoData dialogo;
    public DialogoData dialogoEspecial;

    [Header("Objeto requerido")]
    public bool requiereObjeto = false;
    public ObjetoData objetoRequerido;

    [Header("Objeto que se activa al terminar el diálogo normal")]
    public GameObject objetoAActivar;
    private bool objetoYaActivado = false;

    [Header("Notificar misión al terminar diálogo")]
    public bool notificarMision = false; // Activa solo en NPCs que completan misión

    [Header("Referencias")]
    public DialogoManager dialogoManager;

    private bool jugadorDentro = false;

    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogoManager.dialogoActivo)
                IniciarDialogoSegunEstado();
        }
    }

    void IniciarDialogoSegunEstado()
    {
        if (!requiereObjeto)
        {
            if (objetoAActivar != null && !objetoYaActivado)
                dialogoManager.alTerminarDialogo += ActivarObjeto;

            if (notificarMision)
            {
                dialogoManager.alTerminarDialogo += NotificarMision;
            }

            dialogoManager.IniciarDialogo(dialogo);
            return;
        }

        if (InventarioManager.Instance.TieneObjeto(objetoRequerido))
        {
            if (notificarMision)
                dialogoManager.alTerminarDialogo += NotificarMision;

            dialogoManager.IniciarDialogo(dialogoEspecial);
        }
        else
        {
            dialogoManager.IniciarDialogo(dialogo);
        }
    }

    void NotificarMision()
    {
        if (MisionManager.Instance != null)
            MisionManager.Instance.NotificarDialogoTerminado(gameObject.name);
    }

    void ActivarObjeto()
    {
        objetoAActivar.SetActive(true);
        objetoYaActivado = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorDentro = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            jugadorDentro = false;
    }
}