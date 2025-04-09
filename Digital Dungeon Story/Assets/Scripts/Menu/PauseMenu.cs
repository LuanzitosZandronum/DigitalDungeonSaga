using UnityEngine;
using UnityEngine.SceneManagement;

//public class MainMenuManager : MonoBehaviour
public class PauseMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        SceneManager.LoadScene("GameplayTest");
    }

    public void Inventory()
    {
    }

    public void Skills()
    {
    }

    public void Status()
    {
    }

    public void Settings()
    {
    }
    
    public void ReturnMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
