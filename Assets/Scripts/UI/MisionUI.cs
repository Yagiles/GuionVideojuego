using UnityEngine;
using TMPro;

public class MisionUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelMision;
    public TMP_Text textoMision;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        GameObject canvas = GameObject.Find("CanvasMisiones");
        if (canvas != null)
            DontDestroyOnLoad(canvas);
    }

    void Start()
    {
        RefrescarUI();
    }

    public void RefrescarUI()
    {
        if (MisionManager.Instance == null) return;

        MisionData mision = MisionManager.Instance.GetMisionActual();

        if (mision == null || string.IsNullOrEmpty(mision.tituloMision))
        {
            panelMision.SetActive(false);
            return;
        }

        panelMision.SetActive(true);

        // Si es secuencia de diálogos, muestra con quién hablar ahora
        string npcActual = MisionManager.Instance.GetNPCActual();
        if (npcActual != null)
            textoMision.text = "" + mision.tituloMision + "\nHabla con: " + npcActual;
        else
            textoMision.text = "" + mision.tituloMision + "\n" + mision.descripcion;
    }
}