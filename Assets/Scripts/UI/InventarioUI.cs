using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InventarioUI : MonoBehaviour
{
    public static InventarioUI Instance; // ← añade esta línea

    [Header("Slots (arrastrar los 4 Images)")]
    public Image[] iconosSlots = new Image[4];
    public Sprite spriteVacio;

    [Header("Popup descripción")]
    public GameObject panelPopup;
    public TMP_Text textoNombrePopup;
    public TMP_Text textoDescripcionPopup;

    private KeyCode[] teclas = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
    private GameObject canvasInventario;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasInventario = GameObject.Find("CanvasInventario");
        if (canvasInventario != null)
        {
            DontDestroyOnLoad(canvasInventario);
            canvasInventario.SetActive(false);
        }
    }

    void Update()
    {
        DialogoManager dm = FindFirstObjectByType<DialogoManager>();
        if (dm != null && dm.dialogoActivo) return;

        for (int i = 0; i < teclas.Length; i++)
        {
            if (Input.GetKeyDown(teclas[i]))
                ManejarTecla(i);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && panelPopup.activeSelf)
            panelPopup.SetActive(false);
    }

    void ManejarTecla(int slot)
    {
        ObjetoData objeto = InventarioManager.Instance.GetObjeto(slot);

        if (objeto == null)
        {
            panelPopup.SetActive(false);
            return;
        }

        if (panelPopup.activeSelf && textoNombrePopup.text == objeto.nombreObjeto)
        {
            panelPopup.SetActive(false);
            return;
        }

        textoNombrePopup.text = objeto.nombreObjeto;
        textoDescripcionPopup.text = objeto.descripcion;
        panelPopup.SetActive(true);
    }

    public void RefrescarUI()
    {
        if (canvasInventario != null && !canvasInventario.activeSelf)
            canvasInventario.SetActive(true);

        for (int i = 0; i < iconosSlots.Length; i++)
        {
            ObjetoData objeto = InventarioManager.Instance.GetObjeto(i);
            if (objeto != null)
            {
                iconosSlots[i].sprite = objeto.icono;
                iconosSlots[i].color = Color.white; // Muestra el sprite con colores reales
            }
            else
            {
                iconosSlots[i].sprite = spriteVacio;
                iconosSlots[i].color = new Color(0.3f, 0.15f, 0.05f, 1f); // Marrón oscuro para vacío
            }
        }
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
        RefrescarUI();
    }
}