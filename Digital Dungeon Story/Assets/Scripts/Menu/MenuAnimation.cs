using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MenuAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // === CONSTANTES DE SOM ===
    // Declaramos os nomes dos sons aqui, como strings literais, para serem reaproveitados.
    private const string HOVER_SOUND_NAME = "MenuHover";
    private const string CLICK_SOUND_NAME = "MenuClick";

    // Cooldown específico para o som de hover (0.15s é um bom ponto de partida para evitar repetição excessiva)
    private const float HOVER_COOLDOWN = 0.15f;

    // O componente TextMeshProUGUI que será animado
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

        // Configuração do Brilho
        if (glowImage != null)
        {
            glowImage.gameObject.SetActive(false);
        }

        // Nenhuma lógica de áudio é necessária aqui, o AudioManager lida com isso.
    }

    // =======================================================
    // HANDLERS DE EVENTOS DO MOUSE
    // =======================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1. Efeito Visual: Ativar Brilho
        if (glowImage != null)
        {
            glowImage.gameObject.SetActive(true);
        }

        // 2. Efeito Sonoro: Tocar som de Hover
        if (AudioManager.Instance != null)
        {
            // Chamada direta com a string constante e cooldown
            AudioManager.Instance.PlaySFXByName(HOVER_SOUND_NAME, HOVER_COOLDOWN);
        }

        // 3. Animação de Fonte
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }
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
            // Chamada direta com a string constante (usa o default cooldown)
            AudioManager.Instance.PlaySFXByName(CLICK_SOUND_NAME);
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