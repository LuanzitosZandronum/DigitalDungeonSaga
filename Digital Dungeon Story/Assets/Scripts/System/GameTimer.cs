using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Tooltip("Referência ao componente TextMeshProUGUI na UI para exibir o tempo.")]
    public TextMeshProUGUI timerText;

    private float elapsedTime = 0f;

    private bool isTimerRunning = false;

    private const string TARGET_SCENE = "GameplayTest";


    void Awake()
    {
        if (timerText == null)
        {
            Debug.LogError("GameTimer: O campo 'Timer Text' está vazio! Por favor, atribua um TextMeshProUGUI no Inspector.");
            enabled = false;
            return;
        }

        CheckSceneAndStartTimer();

        timerText.text = "00:00";
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            UpdateTimerDisplay(elapsedTime);
        }
    }

    public void SetTimerRunning(bool running)
    {
        isTimerRunning = running;
    }

    private void CheckSceneAndStartTimer()
    {
        if (SceneManager.GetActiveScene().name == TARGET_SCENE)
        {
            SetTimerRunning(true);
            Debug.Log("Timer iniciado na cena " + TARGET_SCENE);
        }
        else
        {
            SetTimerRunning(false);
            Debug.Log("Timer desativado. Cena atual: " + SceneManager.GetActiveScene().name);
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}