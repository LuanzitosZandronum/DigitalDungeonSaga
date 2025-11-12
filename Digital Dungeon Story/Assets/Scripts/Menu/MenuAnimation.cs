using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MenuAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const string HOVER_SOUND_NAME = "MenuHover";
    private const string CLICK_SOUND_NAME = "MenuClick";
    private const float HOVER_COOLDOWN = 0.15f;

    private TextMeshProUGUI buttonText;


    [Header("Efeitos Visuais")]
    [Tooltip("O GameObject (Texto) que contém os chevrons ou outro indicador visual. Será ativado no hover.")]
    public GameObject chevronIndicator;

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

        ResetState(true);
    }

    public void ResetState(bool instant = false)
    {
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }

        if (chevronIndicator != null)
        {
            chevronIndicator.SetActive(false);
        }

        if (instant && buttonText != null)
        {
            buttonText.fontSize = baseFontSize;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (chevronIndicator != null)
        {
            chevronIndicator.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
        }

        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }

        activeAnimation = StartCoroutine(AnimateFontSize(targetFontSize));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (chevronIndicator != null)
        {
            chevronIndicator.SetActive(false);
        }

        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
        }

        activeAnimation = StartCoroutine(AnimateFontSize(baseFontSize));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
        {
        }
    }


    private IEnumerator AnimateFontSize(int targetSize)
    {
        float startTime = Time.time;
        float elapsed = 0f;
        float startSize = buttonText.fontSize;

        if (buttonText == null) yield break;

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