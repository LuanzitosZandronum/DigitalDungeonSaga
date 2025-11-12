using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum PanelType
{
    Dialogue,
    Description
}

[System.Serializable]
public class DialogueStep
{
    public PanelType type;
    [TextArea(3, 10)]
    public string text;
    
    [Tooltip("Tempo (em segundos) de pausa entre a exibição de cada caractere.")]
    public float delayPerCharacter = 0.03f;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Painéis de UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;

    [Header("Configurações")]
    public KeyCode advanceKey = KeyCode.E;

    private List<DialogueStep> currentSequence;
    private int currentStepIndex = 0;
    private bool isDialogueActive = false;
    
    private bool ignoreInitialInput = false;

    private Coroutine typingCoroutine;
    private bool isTyping = false; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }

    void Update()
    {
        if (!isDialogueActive) return;

        if (ignoreInitialInput)
        {
            if (Input.GetKeyDown(advanceKey))
            {
            }
            ignoreInitialInput = false; 
            return;
        }

        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping)
            {
                SkipTypingAnimation();
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    public void StartDialogue(List<DialogueStep> sequence)
    {
        if (isDialogueActive) return;
        
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentSequence = sequence;
        currentStepIndex = 0;
        isDialogueActive = true;
        
        ignoreInitialInput = true;

        DisplayCurrentStep();

        Debug.Log("Diálogo Iniciado. Exibindo passo 0.");
    }

    private void AdvanceDialogue()
    {
        if (isTyping) return;

        currentStepIndex++;

        if (currentStepIndex < currentSequence.Count)
        {
            DisplayCurrentStep();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayCurrentStep()
    {
        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);

        DialogueStep step = currentSequence[currentStepIndex];

        if (step.type == PanelType.Dialogue)
        {
            dialoguePanel.SetActive(true);
            
            string formattedText = "\"" + step.text + "\"";
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypewriteText(formattedText, step.delayPerCharacter));
        }
        else if (step.type == PanelType.Description)
        {
            descriptionPanel.SetActive(true);
            descriptionText.text = step.text;
        }
    }

    private IEnumerator TypewriteText(string textToType, float delay)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay);
        }
        
        isTyping = false;
        typingCoroutine = null;
    }

    private void SkipTypingAnimation()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        DialogueStep step = currentSequence[currentStepIndex];
        if (step.type == PanelType.Dialogue)
        {
            dialogueText.text = "\"" + step.text + "\"";
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);

        Debug.Log("Diálogo Encerrado.");
    }

    public bool IsDialogueActive => isDialogueActive;
}