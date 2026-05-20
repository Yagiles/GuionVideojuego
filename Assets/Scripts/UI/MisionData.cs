using UnityEngine;

public enum TipoCompletado
{
    AlCambiarEscena,
    AlTerminarDialogo,
    AlRecogerObjeto,
    SecuenciaDialogos,
    SecuenciaDialogosSinOrden
}

[CreateAssetMenu(fileName = "NuevaMision", menuName = "Misiones/Mision")]
public class MisionData : ScriptableObject
{
    public string tituloMision;

    [TextArea(2, 4)]
    public string descripcion;

    public TipoCompletado tipoCompletado;
    [Header("Al completar")]
    public bool activarSiguienteAlCompletar = false;

    public ObjetoData[] objetosRequeridos;

    public string[] nombresNPCsEnOrden;
}