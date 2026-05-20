using UnityEngine;
using System.Collections.Generic;

public class InventarioManager : MonoBehaviour
{
    public static InventarioManager Instance;
    [SerializeField] private ObjetoData[] objetos = new ObjetoData[4]; // 4 slots fijos

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

    void Start()
    {
        // Start se ejecuta después de que todos los objetos están en escena
        InventarioUI ui = FindFirstObjectByType<InventarioUI>();
        if (ui != null)
            DontDestroyOnLoad(ui.gameObject.transform.root.gameObject);
        else
            Debug.Log("InventarioUI no encontrado");
    }

    public void AñadirObjeto(ObjetoData objeto)
    {
        for (int i = 0; i < objetos.Length; i++)
        {
            if (objetos[i] == null)
            {
                objetos[i] = objeto;
                Debug.Log("Objeto añadido en slot " + i + ": " + objeto.nombreObjeto);
                return;
            }
        }
        Debug.Log("Inventario lleno");
    }

    public bool TieneObjeto(ObjetoData objeto)
    {
        for (int i = 0; i < objetos.Length; i++)
        {
            if (objetos[i] == objeto) return true;
        }
        return false;
    }

    public ObjetoData GetObjeto(int slot)
    {
        return objetos[slot];
    }
}