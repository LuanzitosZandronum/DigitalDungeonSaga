using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Adicione se for carregar cenas

// CLASSE RENOMEADA: Agora é MainMenuEffect
public class MainMenuEffect : MonoBehaviour
{
    [Header("Configurações do Efeito Dilate")]
    [Tooltip("Valor de Dilate normal dos textos (geralmente 0).")]
    public float baseDilate = 0f;

    [Tooltip("Valor de Dilate máximo (tremor) quando o botão é pressionado (Ex: 0.5 a 1.0).")]
    public float targetDilate = 0.8f;

    [Tooltip("Duração total do efeito Dilate antes de retornar ao normal.")]
    public float dilateDuration = 1.5f;

    // Lista para armazenar todos os textos TMP na cena (ou no Canvas)
    private List<TextMeshProUGUI> allMenuTexts = new List<TextMeshProUGUI>();

    // Variáveis de controle para evitar múltiplas animações
    private Coroutine dilateCoroutine;
    private bool isEffectActive = false;

    void Awake()
    {
        // Encontra TODOS os TextMeshProUGUI na cena inteira.
        // Se você quiser limitar apenas aos textos dentro do Canvas, use: 
        // GetComponentsInChildren<TextMeshProUGUI>();
        allMenuTexts.AddRange(FindObjectsOfType<TextMeshProUGUI>());

        // MENSAGEM DE DEBUG ATUALIZADA
        Debug.Log($"MainMenuEffect encontrou {allMenuTexts.Count} textos TMP.");
    }

    // =================================================================
    // FUNÇÕES CHAMADAS PELOS BOTÕES
    // =================================================================

    // Chamada por StartButton.OnClick()
    public void OnStartButtonPressed()
    {
        if (!isEffectActive)
        {
            if (dilateCoroutine != null) StopCoroutine(dilateCoroutine);
            dilateCoroutine = StartCoroutine(AnimateDilateAndLoad("GameplayTest"));
        }
    }

    // Chamada por QuitButton.OnClick()
    public void OnQuitButtonPressed()
    {
        if (!isEffectActive)
        {
            if (dilateCoroutine != null) StopCoroutine(dilateCoroutine);
            // Chama a Coroutine com null, indicando que não há cena para carregar
            dilateCoroutine = StartCoroutine(AnimateDilateAndLoad(null));
        }
    }

    // =================================================================
    // LÓGICA DE ANIMAÇÃO E AÇÃO
    // =================================================================

    private IEnumerator AnimateDilateAndLoad(string sceneToLoad)
    {
        isEffectActive = true;

        // 1. ANIMAÇÃO DE SUBIDA DO DILATE (RÁPIDA)
        yield return StartCoroutine(AnimateDilateValue(targetDilate, dilateDuration / 2f));

        // 2. PAUSA/BRILHO MÁXIMO (INSTANTÂNEO)
        yield return new WaitForSeconds(0.05f);

        // 3. ANIMAÇÃO DE DESCIDA DO DILATE (RÁPIDA)
        yield return StartCoroutine(AnimateDilateValue(baseDilate, dilateDuration / 2f));

        // 4. AÇÃO FINAL
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // Se for o botão Start, carrega a cena
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            // Se for o botão Quit, fecha o aplicativo
            Debug.Log("Saindo do Jogo...");
            Application.Quit();
        }

        isEffectActive = false;
    }

    // Função genérica para animar o Dilate em todos os textos
    private IEnumerator AnimateDilateValue(float targetValue, float duration)
    {
        float startTime = Time.time;
        float elapsed = 0f;

        // Armazena os valores de dilate iniciais para cada texto
        List<float> startDilateValues = new List<float>();
        foreach (var text in allMenuTexts)
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

            // Usa SmoothStep para uma transição suave
            float smoothT = t * t * (3f - 2f * t);

            for (int i = 0; i < allMenuTexts.Count; i++)
            {
                var text = allMenuTexts[i];
                if (text != null)
                {
                    float newDilate = Mathf.Lerp(startDilateValues[i], targetValue, smoothT);

                    // IMPORTANTE: Modificar o material da fonte!
                    text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, newDilate);
                }
            }
            yield return null;
        }

        // Garante que o valor final seja exato
        foreach (var text in allMenuTexts)
        {
            if (text != null)
            {
                text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, targetValue);
            }
        }
    }
}