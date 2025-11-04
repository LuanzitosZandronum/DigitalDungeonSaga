using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro; // Usando TextMeshPro para textos de UI

public class MenuAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // O componente TextMeshProUGUI que será animado
    // Tentar obter TextMeshProUGUI (para textos modernos)
    private TextMeshProUGUI buttonText;

    [Header("Configurações de Animação")]
    [Tooltip("Tamanho da fonte quando o mouse NÃO está sobre o botão.")]
    // TMP usa float para tamanho de fonte, mas usaremos int para clareza no Inspector
    public int baseFontSize = 25;

    [Tooltip("Tamanho máximo da fonte quando o mouse ESTÁ sobre o botão.")]
    public int targetFontSize = 30;

    [Tooltip("Duração da animação em segundos.")]
    public float animationDuration = 0.15f;

    private Coroutine activeAnimation;

    void Awake()
    {
        // Tenta obter o componente TextMeshProUGUI no GameObject (ou nos filhos)
        // Isso deve resolver o NullReferenceException
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            Debug.LogError("MenuAnimation requer um componente TextMeshProUGUI no botão ou em um filho.");
            enabled = false;
            return;
        }

        // Define o tamanho base inicial (TMP usa float)
        buttonText.fontSize = baseFontSize;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Se houver uma animação em andamento, pare-a para iniciar a nova
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }

        // Inicia a animação de aumento
        activeAnimation = StartCoroutine(AnimateFontSize(targetFontSize));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Se houver uma animação em andamento, pare-a para iniciar a nova
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }

        // Inicia a animação de retorno ao tamanho base
        activeAnimation = StartCoroutine(AnimateFontSize(baseFontSize));
    }

    // Coroutine que anima suavemente o tamanho da fonte
    private IEnumerator AnimateFontSize(int targetSize)
    {
        float startTime = Time.time;
        float elapsed = 0f;

        // Lê o tamanho de início como float para interpolação suave
        float startSize = buttonText.fontSize;

        while (elapsed < animationDuration)
        {
            elapsed = Time.time - startTime;
            float t = elapsed / animationDuration;

            // Usa SmoothStep para uma transição mais orgânica (começa e termina suave)
            float smoothT = t * t * (3f - 2f * t);

            // Interpola o tamanho da fonte (TMP usa float)
            buttonText.fontSize = Mathf.Lerp(startSize, targetSize, smoothT);

            yield return null;
        }

        // Garante que o tamanho final seja exatamente o alvo
        buttonText.fontSize = targetSize;
        activeAnimation = null;
    }
}