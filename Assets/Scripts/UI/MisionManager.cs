using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MisionManager : MonoBehaviour
{
    public static MisionManager Instance;

    [Header("Lista de misiones en orden")]
    public List<MisionData> misiones;

    private int indiceMisionActual = 0;
    private bool primeraEscena = true;

    // Para misiones tipo SecuenciaDialogos
    private int indiceNPCActual = 0;

    // Para misiones tipo AlRecogerObjeto con varios objetos
    private List<ObjetoData> objetosRecogidosEnMision = new List<ObjetoData>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnScenaCargada;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnScenaCargada;
    }

    void OnScenaCargada(Scene escena, LoadSceneMode mode)
    {
        if (primeraEscena)
        {
            primeraEscena = false;
            ActualizarUI();
            return;
        }

        if (indiceMisionActual >= misiones.Count) return;

        MisionData mision = misiones[indiceMisionActual];

        if (mision.tipoCompletado == TipoCompletado.AlCambiarEscena)
            CompletarMisionActual();
        else
            ActualizarUI();
    }

    // Llamado desde DialogoTrigger al terminar un diálogo
    public void NotificarDialogoTerminado(string nombreNPC)
    {
        if (indiceMisionActual >= misiones.Count) return;

        MisionData mision = misiones[indiceMisionActual];

        if (mision.tipoCompletado == TipoCompletado.AlTerminarDialogo)
        {
            CompletarMisionActual();
        }
        else if (mision.tipoCompletado == TipoCompletado.SecuenciaDialogos)
        {
            if (indiceNPCActual < mision.nombresNPCsEnOrden.Length)
            {
                if (mision.nombresNPCsEnOrden[indiceNPCActual] == nombreNPC)
                {
                    indiceNPCActual++;
                    // Si hemos hablado con todos, completar misión
                    if (indiceNPCActual >= mision.nombresNPCsEnOrden.Length)
                    {
                        indiceNPCActual = 0;
                        CompletarMisionActual();
                    }
                    else
                    {
                        // Actualiza el texto para indicar con quién hablar ahora
                        ActualizarUI();
                    }
                }
            }
        }
    }

    // Llamado desde ObjetoRecolectable al recoger un objeto
    public void NotificarObjetoRecogido(ObjetoData objeto)
    {
        if (indiceMisionActual >= misiones.Count) return;

        MisionData mision = misiones[indiceMisionActual];

        if (mision.tipoCompletado != TipoCompletado.AlRecogerObjeto) return;

        if (!objetosRecogidosEnMision.Contains(objeto))
            objetosRecogidosEnMision.Add(objeto);

        // Comprueba si se han recogido todos los objetos requeridos
        bool todosRecogidos = true;
        foreach (ObjetoData obj in mision.objetosRequeridos)
        {
            if (!objetosRecogidosEnMision.Contains(obj))
            {
                todosRecogidos = false;
                break;
            }
        }

        if (todosRecogidos)
        {
            objetosRecogidosEnMision.Clear();
            CompletarMisionActual();
        }
    }

    public void CompletarMisionActual()
    {
        if (indiceMisionActual >= misiones.Count) return;
        indiceMisionActual++;
        ActualizarUI();
    }

    public MisionData GetMisionActual()
    {
        if (indiceMisionActual >= misiones.Count) return null;
        return misiones[indiceMisionActual];
    }

    // Para misiones de secuencia, devuelve con quién hay que hablar ahora
    public string GetNPCActual()
    {
        if (indiceMisionActual >= misiones.Count) return null;
        MisionData mision = misiones[indiceMisionActual];
        if (mision.tipoCompletado != TipoCompletado.SecuenciaDialogos) return null;
        if (indiceNPCActual >= mision.nombresNPCsEnOrden.Length) return null;
        return mision.nombresNPCsEnOrden[indiceNPCActual];
    }

    void ActualizarUI()
    {
        MisionUI ui = FindFirstObjectByType<MisionUI>();
        if (ui != null)
            ui.RefrescarUI();
    }
}