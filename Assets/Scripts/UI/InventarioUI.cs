using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // ← IMPORTANTE: Añade esta línea para poder usar Corrutinas

public class InventarioUI : MonoBehaviour
{
    public static InventarioUI Instance;

    [Header("Slots (arrastrar los 4 Images)")]
    public Image[] iconosSlots = new Image[4];
    public Sprite spriteVacio;

    [Header("Popup descripción")]
    public GameObject panelPopup;
    public TMP_Text textoNombrePopup;
    public TMP_Text textoDescripcionPopup;

    private KeyCode[] teclas = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    [Header("Asigna el Canvas directamente aquí en el Inspector")]
    [SerializeField] private GameObject canvasInventario;

    void Awake()
    {
        if (Instance != null)
        {
            if (canvasInventario != null)
            {
                Destroy(canvasInventario);
            }
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasInventario != null)
        {
            DontDestroyOnLoad(canvasInventario);
            canvasInventario.SetActive(false);
        }
        else
        {
            canvasInventario = GameObject.Find("CanvasInventario");
            if (canvasInventario != null)
            {
                DontDestroyOnLoad(canvasInventario);
                canvasInventario.SetActive(false);
            }
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

        if (Input.GetKeyDown(KeyCode.Escape) && panelPopup != null && panelPopup.activeSelf)
            panelPopup.SetActive(false);
    }

    void ManejarTecla(int slot)
    {
        if (InventarioManager.Instance == null) return;

        ObjetoData objeto = InventarioManager.Instance.GetObjeto(slot);

        if (objeto == null)
        {
            if (panelPopup != null) panelPopup.SetActive(false);
            return;
        }

        if (panelPopup != null && panelPopup.activeSelf && textoNombrePopup != null && textoNombrePopup.text == objeto.nombreObjeto)
        {
            panelPopup.SetActive(false);
            return;
        }

        if (textoNombrePopup != null) textoNombrePopup.text = objeto.nombreObjeto;
        if (textoDescripcionPopup != null) textoDescripcionPopup.text = objeto.descripcion;
        if (panelPopup != null) panelPopup.SetActive(true);
    }

    public void RefrescarUI()
    {
        if (InventarioManager.Instance == null)
        {
            Debug.LogWarning("InventarioManager no encontrado al refrescar UI");
            return;
        }

        if (canvasInventario != null && !canvasInventario.activeSelf)
            canvasInventario.SetActive(true);

        for (int i = 0; i < iconosSlots.Length; i++)
        {
            if (iconosSlots[i] == null)
            {
                Debug.LogError($"El icono del slot {i} es nulo en RefrescarUI.");
                continue;
            }

            ObjetoData objeto = InventarioManager.Instance.GetObjeto(i);
            if (objeto != null)
            {
                iconosSlots[i].sprite = objeto.icono;
                iconosSlots[i].color = Color.white;
            }
            else
            {
                iconosSlots[i].sprite = spriteVacio;
                iconosSlots[i].color = new Color(0.3f, 0.15f, 0.05f, 1f);
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
        // En lugar de llamar directo a RefrescarUI, iniciamos la espera controlada
        StartCoroutine(EsperarYRefrescarSincronizado());
    }

    // Esta corrutina obliga a Unity a terminar de procesar toda la carga antes de pintar los sprites
    private IEnumerator EsperarYRefrescarSincronizado()
    {
        // Esperamos a que termine el frame actual de carga
        yield return new WaitForEndOfFrame();

        // Adicionalmente, si tu InventarioManager tarda un pelín en cargar de un archivo o PlayerPrefs, 
        // puedes descomentar la siguiente línea para esperar un cuadro extra:
        // yield return null;

        RefrescarUI();
    }
}