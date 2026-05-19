using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public string nombreSiguienteEscena;

    [Header("Spawn")]
    public string nombreSpawnDestino;

    [Header("Guia")]
    public bool modoVueltaAlEntrar = false;

    [Header("Condicion dialogo")]
    public bool requiereDialogoPrevio = false;
    public DialogoData[] dialogosNecesarios;

    [Header("Fundido")]
    public CanvasGroup pantallaNegra;
    public float duracionFundido = 1f;

    private bool cambiandoEscena = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cambiandoEscena) return;

        if (collision.CompareTag("Player"))
        {
            DialogoManager dialogoManager = FindFirstObjectByType<DialogoManager>();

            if (dialogoManager != null && dialogoManager.dialogoActivo)
            {
                Debug.Log("No puedes cambiar de escena mientras hay un dialogo activo");
                return;
            }

            if (requiereDialogoPrevio)
            {
                if (EstadoDialogos.instancia == null)
                {
                    Debug.LogWarning("No existe EstadoDialogos");
                    return;
                }

                if (dialogosNecesarios == null || dialogosNecesarios.Length == 0)
                {
                    Debug.LogWarning("Falta asignar al menos un DialogoData necesario");
                    return;
                }

                for (int i = 0; i < dialogosNecesarios.Length; i++)
                {
                    if (dialogosNecesarios[i] == null)
                    {
                        Debug.LogWarning("Hay un DialogoData necesario sin asignar en la posicion " + i);
                        return;
                    }

                    if (!EstadoDialogos.instancia.HaHabladoCon(dialogosNecesarios[i].name))
                    {
                        Debug.Log("Todavia no se ha completado el dialogo necesario: " + dialogosNecesarios[i].name);
                        return;
                    }
                }
            }

            StartCoroutine(CambiarEscena());
        }
    }

    private IEnumerator CambiarEscena()
    {
        cambiandoEscena = true;

        DatosCambioEscena.spawnDestino = nombreSpawnDestino;
        DatosCambioEscena.modoVuelta = modoVueltaAlEntrar;

        float tiempo = 0f;

        while (tiempo < duracionFundido)
        {
            tiempo += Time.deltaTime;

            if (pantallaNegra != null)
            {
                pantallaNegra.alpha = tiempo / duracionFundido;
            }

            yield return null;
        }

        SceneManager.LoadScene(nombreSiguienteEscena);
    }
}