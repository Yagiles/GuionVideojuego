using UnityEngine;

public enum TipoCompletado
{
    AlCambiarEscena,
    AlTerminarDialogo,
    AlRecogerObjeto,
    SecuenciaDialogos
}

[CreateAssetMenu(fileName = "NuevaMision", menuName = "Misiones/Mision")]
public class MisionData : ScriptableObject
{
    public string tituloMision;
    [TextArea(2, 4)]
    public string descripcion;

    public TipoCompletado tipoCompletado;

    // Para tipo AlRecogerObjeto
    public ObjetoData[] objetosRequeridos;

    // Para tipo SecuenciaDialogos
    public string[] nombresNPCsEnOrden;
}