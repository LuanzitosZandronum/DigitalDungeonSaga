using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // Necessário para UnityEditor.EditorApplication.ExitPlaymode()
#endif

public class MainMenu : MonoBehaviour
{
    [Header("Configurações do Efeito Dilate (Tela Cheia)")]
    [Tooltip("Valor de Dilate normal dos textos (geralmente 0).")]
    public float baseDilate = 0f;

    [Tooltip("Valor de Dilate máximo (tremor) quando o botão é pressionado (Ex: 0.5 a 1.0).")]
    public float targetDilate = 0.8f;

    [Tooltip("Duração total do efeito Dilate antes de retornar ao normal.")]
    public float dilateDuration = 1.5f;

    [Header("Configurações de Cena")]
    [Tooltip("O nome exato da cena do jogo a ser carregada.")]
    public string gameplaySceneName = "GameplayTest";


    // Lista para armazenar todos os textos TMP na cena
    private List<TextMeshProUGUI> allSceneTexts = new List<TextMeshProUGUI>();

    private Coroutine dilateCoroutine;
    private bool isEffectActive = false;

    void Awake()
    {
        // Encontra TODOS os TextMeshProUGUI na cena inteira para o efeito de Dilate
        allSceneTexts.AddRange(FindObjectsOfType<TextMeshProUGUI>());

        // Ajustado para o nome da sua classe
        Debug.Log($"MainMenu encontrou {allSceneTexts.Count} textos TMP.");
    }

    // =================================================================
    // FUNÇÕES PÚBLICAS CHAMADAS PELOS BOTÕES
    // =================================================================
    
    /// <summary>
    /// Inicia o efeito Dilate e carrega a cena de Gameplay.
    /// </summary>
    public void StartGame()
    {
        // Chama a transição com o nome da cena
        StartTransition("GameplayTest");
    }
    
    /// <summary>
    /// Inicia o efeito Dilate e encerra a aplicação.
    /// </summary>
    public void QuitGame()
    {
        // Chama a transição com o identificador "Quit"
        StartTransition("Quit");
    }

    // =================================================================
    // FUNÇÃO CORE DE TRANSIÇÃO
    // =================================================================

    // Função universal para iniciar a transição com segurança
    public void StartTransition(string sceneName)
    {
        if (!isEffectActive)
        {
            if (dilateCoroutine != null) StopCoroutine(dilateCoroutine);
            dilateCoroutine = StartCoroutine(AnimateDilateAndLoad(sceneName));
        }
    }

    // =================================================================
    // LÓGICA DE ANIMAÇÃO E AÇÃO
    // =================================================================

    private IEnumerator AnimateDilateAndLoad(string sceneToLoad)
    {
        isEffectActive = true;

        // 1. ANIMAÇÃO DE SUBIDA DO DILATE
        yield return StartCoroutine(AnimateDilateValue(targetDilate, dilateDuration / 2f));

        // 2. PAUSA/BRILHO MÁXIMO (INSTANTÂNEO)
        yield return new WaitForSeconds(0.05f);

        // 3. ANIMAÇÃO DE DESCIDA DO DILATE
        yield return StartCoroutine(AnimateDilateValue(baseDilate, dilateDuration / 2f));

        // 4. AÇÃO FINAL
        if (!string.IsNullOrEmpty(sceneToLoad) && sceneToLoad != "Quit")
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else if (sceneToLoad == "Quit")
        {
            Debug.Log("Saindo do Jogo...");
            Application.Quit();

#if UNITY_EDITOR
            // Usa o código do UnityEditor para parar o modo de jogo
            EditorApplication.ExitPlaymode();
#endif
        }

        isEffectActive = false;
    }

    // Função genérica para animar o Dilate em todos os textos
    private IEnumerator AnimateDilateValue(float targetValue, float duration)
    {
        float startTime = Time.time;
        float elapsed = 0f;

        List<float> startDilateValues = new List<float>();
        foreach (var text in allSceneTexts)
        {
            if (text != null)
            {
                // Acessa o valor FaceDilate do material da fonte
                startDilateValues.Add(text.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate));
            }
        }

        while (elapsed < duration)
        {
            elapsed = Time.time - startTime;
            float t = elapsed / duration;
            float smoothT = t * t * (3f - 2f * t);

            for (int i = 0; i < allSceneTexts.Count; i++)
            {
                var text = allSceneTexts[i];
                if (text != null)
                {
                    float newDilate = Mathf.Lerp(startDilateValues[i], targetValue, smoothT);
                    text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, newDilate);
                }
            }
            yield return null;
        }

        foreach (var text in allSceneTexts)
        {
            if (text != null)
            {
                text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, targetValue);
            }
        }
    }
}