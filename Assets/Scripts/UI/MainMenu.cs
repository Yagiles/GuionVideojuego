using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panel de controles")]
    public GameObject panelControles;
    public void StartGame()
    {
        SceneManager.LoadScene("Escena 0(Puerta)");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void AbrirControles()
    {
        panelControles.SetActive(true);
    }

    public void CerrarControles()
    {
        panelControles.SetActive(false);
    }

}