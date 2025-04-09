using UnityEngine;
using UnityEngine.SceneManagement;

//public class MainMenuManager : MonoBehaviour
public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameplayTest");
    }

    public void Quit()
    {
        //Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode(); // Para o Play Mode no Editor
        #else
        Application.Quit(); // Fecha o jogo no build
        #endif
    }
}
