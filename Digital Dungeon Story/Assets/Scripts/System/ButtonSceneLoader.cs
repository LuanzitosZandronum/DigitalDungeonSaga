using UnityEngine;

public class ButtonSceneLoader : MonoBehaviour
{
    [Header("Configuração de Cena")]
    [Tooltip("O nome exato da cena que este botão deve carregar. Ignorado se 'Is Quit Button' for marcado.")]
    public string targetSceneName;

    [Tooltip("Marque esta caixa se este botão deve fechar o jogo.")]
    public bool isQuitButton = false;

    public void TriggerSceneLoad()
    {
        string sceneToLoad = isQuitButton ? "Quit" : targetSceneName;

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadSceneWithTransition(sceneToLoad);
        }
        else
        {
            Debug.LogError("SceneTransitionManager não foi encontrado na cena!");
        }
    }
}