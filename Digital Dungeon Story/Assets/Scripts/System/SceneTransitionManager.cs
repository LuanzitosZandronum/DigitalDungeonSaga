using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Configurações do Efeito Dilate (Tela Cheia)")]
    public float baseDilate = 0f;
    public float targetDilate = 0.8f;
    public float dilateDuration = 1.5f;

    private List<TextMeshProUGUI> allSceneTexts = new List<TextMeshProUGUI>();
    private Coroutine dilateCoroutine;
    private bool isEffectActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        allSceneTexts.AddRange(FindObjectsOfType<TextMeshProUGUI>());
    }

    public void LoadSceneWithTransition(string sceneName)
    {
        if (!isEffectActive)
        {
            if (dilateCoroutine != null) StopCoroutine(dilateCoroutine);
            dilateCoroutine = StartCoroutine(AnimateDilateAndLoad(sceneName));
        }
    }

    private IEnumerator AnimateDilateAndLoad(string sceneToLoad)
    {
        isEffectActive = true;

        yield return StartCoroutine(AnimateDilateValue(targetDilate, dilateDuration / 2f));
        yield return new WaitForSeconds(0.05f);

        if (sceneToLoad == "Quit")
        {
            Debug.Log("Saindo do Jogo...");
            Application.Quit();

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            yield break;
        }

        yield return StartCoroutine(AnimateDilateValue(baseDilate, dilateDuration / 2f));

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }

        isEffectActive = false;
    }

    private IEnumerator AnimateDilateValue(float targetValue, float duration)
    {
        float startTime = Time.time;
        float elapsed = 0f;

        List<float> startDilateValues = new List<float>();
        foreach (var text in allSceneTexts)
        {
            if (text != null)
            {
                startDilateValues.Add(text.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate));
            }
        }

        if (allSceneTexts.Count == 0) yield break;

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

    public void QuitGame()
    {
        StartCoroutine(QuitSequence());
    }

    private IEnumerator QuitSequence()
    {
        yield return StartCoroutine(AnimateDilateValue(targetDilate, dilateDuration / 2f));
        yield return new WaitForSeconds(0.05f);
        Debug.Log("Saindo do jogo...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
    Application.Quit();
#endif
    }
}