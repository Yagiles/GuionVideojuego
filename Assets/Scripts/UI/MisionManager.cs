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

    private bool misionActiva = false;

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

        if (!misionActiva) return;
        if (indiceMisionActual >= misiones.Count) return;

        MisionData mision = misiones[indiceMisionActual];

        if (mision.tipoCompletado == TipoCompletado.AlCambiarEscena)
            CompletarMisionActual();
        else
            ActualizarUI();
    }

    public void ActivarMisionActual()
    {
        if (indiceMisionActual >= misiones.Count) return;

        misionActiva = true;
        indiceNPCActual = 0;
        objetosRecogidosEnMision.Clear();

        ActualizarUI();
    }

    public void NotificarDialogoTerminado(string nombreNPC)
    {
        if (!misionActiva) return;
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

                    if (indiceNPCActual >= mision.nombresNPCsEnOrden.Length)
                    {
                        CompletarMisionActual();
                    }
                    else
                    {
                        ActualizarUI();
                    }
                }
            }
        }
    }

    public void NotificarObjetoRecogido(ObjetoData objeto)
    {
        if (!misionActiva) return;
        if (indiceMisionActual >= misiones.Count) return;

        MisionData mision = misiones[indiceMisionActual];

        if (mision.tipoCompletado != TipoCompletado.AlRecogerObjeto) return;

        if (!objetosRecogidosEnMision.Contains(objeto))
            objetosRecogidosEnMision.Add(objeto);

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
            CompletarMisionActual();
        }
    }

    public void CompletarMisionActual()
    {
        if (indiceMisionActual >= misiones.Count) return;

        indiceMisionActual++;
        misionActiva = false;

        indiceNPCActual = 0;
        objetosRecogidosEnMision.Clear();

        ActualizarUI();
    }

    public MisionData GetMisionActual()
    {
        if (!misionActiva) return null;
        if (indiceMisionActual >= misiones.Count) return null;

        return misiones[indiceMisionActual];
    }

    public string GetNPCActual()
    {
        if (!misionActiva) return null;
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