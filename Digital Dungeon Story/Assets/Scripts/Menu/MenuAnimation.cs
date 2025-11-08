using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MenuAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // === CONSTANTES DE SOM ===
    private const string HOVER_SOUND_NAME = "MenuHover";
    private const string CLICK_SOUND_NAME = "MenuClick";
    private const float HOVER_COOLDOWN = 0.15f;

    private TextMeshProUGUI buttonText;

    [Header("Efeitos Visuais")]
    [Tooltip("Imagem de brilho (glow) que será ativada e desativada no hover.")]
    public Image glowImage;

    [Tooltip("Tamanho da fonte quando o mouse NÃO está sobre o botão.")]
    public int baseFontSize = 25;

    [Tooltip("Tamanho máximo da fonte quando o mouse ESTÁ sobre o botão.")]
    public int targetFontSize = 30;

    [Tooltip("Duração da animação em segundos.")]
    public float animationDuration = 0.15f;

    private Coroutine activeAnimation;

    void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            Debug.LogError("MenuAnimation requer um componente TextMeshProUGUI no botão ou em um filho.");
            enabled = false;
            return;
        }

        buttonText.fontSize = baseFontSize;

        // Garante que o estado base seja definido ao iniciar
        ResetState(true);
    }

    // NOVO: Função para forçar o botão a retornar ao estado base
    /// <summary>
    /// Força o estado visual do botão para o estado base (tamanho/brilho)
    /// </summary>
    /// <param name="instant">Se verdadeiro, o texto é resetado instantaneamente sem animação.</param>
    public void ResetState(bool instant = false)
    {
        // 1. Para qualquer animação em andamento
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }

        // 2. Garante que o brilho seja desativado
        if (glowImage != null)
        {
            glowImage.gameObject.SetActive(false);
        }

        // 3. Garante o tamanho base da fonte (se instantâneo)
        if (instant && buttonText != null)
        {
            buttonText.fontSize = baseFontSize;
        }
    }


    // =======================================================
    // HANDLERS DE EVENTOS DO MOUSE
    // =======================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1. ANIMAÇÃO DE ENTRADA: Chame o Reset para garantir que a animação anterior pare
        ResetState(false);

        // 2. Efeito Visual: Ativar Brilho
        if (glowImage != null)
        {
            glowImage.gameObject.SetActive(true);
        }

        // 3. Efeito Sonoro: Tocar som de Hover
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXByName(HOVER_SOUND_NAME, HOVER_COOLDOWN);
        }

        // 4. Animação de Fonte
        activeAnimation = StartCoroutine(AnimateFontSize(targetFontSize));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 1. Efeito Visual: Desativar Brilho
        if (glowImage != null)
        {
            glowImage.gameObject.SetActive(false);
        }

        // 2. Animação de Fonte
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }
        activeAnimation = StartCoroutine(AnimateFontSize(baseFontSize));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Efeito Sonoro: Tocar som de Click
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXByName("MenuClick");
        }
    }

    // =======================================================
    // COROUTINE DE ANIMAÇÃO DE FONTE (Mantida)
    // =======================================================

    private IEnumerator AnimateFontSize(int targetSize)
    {
        float startTime = Time.time;
        float elapsed = 0f;
        float startSize = buttonText.fontSize;

        while (elapsed < animationDuration)
        {
            elapsed = Time.time - startTime;
            float t = elapsed / animationDuration;
            float smoothT = t * t * (3f - 2f * t);

            buttonText.fontSize = Mathf.Lerp(startSize, targetSize, smoothT);

            yield return null;
        }

        buttonText.fontSize = targetSize;
        activeAnimation = null;
    }
}