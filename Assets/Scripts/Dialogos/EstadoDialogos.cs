using System.Collections.Generic;
using UnityEngine;

public class EstadoDialogos : MonoBehaviour
{
    public static EstadoDialogos instancia;

    private HashSet<string> dialogosCompletados = new HashSet<string>();

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarcarComoHablado(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            dialogosCompletados.Add(id);
        }
    }

    public bool HaHabladoCon(string id)
    {
        return !string.IsNullOrEmpty(id) && dialogosCompletados.Contains(id);
    }
}