
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

    public GameObject descriptionPanel;   // O objeto DescriptionPanel (Canvas/Pai)
    public TextMeshProUGUI descriptionText; // O TextMeshPro dentro do DescriptionPanel

    [Header("Configurações")]
    public KeyCode advanceKey = KeyCode.E; // Chave para avançar o diálogo

    private List<DialogueStep> currentSequence; // A sequência de falas atual
    private int currentStepIndex = 0;
    private bool isDialogueActive = false;
    
    // CORREÇÃO: Flag para ignorar o input 'E' usado para INICIAR o diálogo no primeiro frame.
    private bool ignoreInitialInput = false;

    // NOVO: Controle da animação
    private Coroutine typingCoroutine;
    private bool isTyping = false; // True enquanto a animação estiver em andamento

    void Awake()
    {
        // Implementação do Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Garante que ambos os painéis estejam inicialmente desativados
        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);
    }

    void Update()
    {
        if (!isDialogueActive) return;

        // =======================================================
        // LÓGICA DE CORREÇÃO DE SALTO: Bloqueia o primeiro input 'E'
        // =======================================================
        if (ignoreInitialInput)
        {
            if (Input.GetKeyDown(advanceKey))
            {
                // Consome o input.
            }
            ignoreInitialInput = false; 
            return;
        }
        // =======================================================

        // Verifica se a chave de avanço foi pressionada (Lógica de Pular/Avançar)
        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping)
            {
                // Se estiver digitando, PULA a animação imediatamente.
                SkipTypingAnimation();
            }
            else
            {
                // Se não estiver digitando, AVANÇA o diálogo.
                AdvanceDialogue();
            }
        }
    }

    /// <summary>
    /// Inicia uma nova sequência de diálogo/texto.
    /// </summary>
    /// <param name="sequence">A lista de DialogueSteps a ser exibida.</param>
    public void StartDialogue(List<DialogueStep> sequence)
    {
        if (isDialogueActive) return; // Evita iniciar um novo se já houver um ativo
        
        // Garante que qualquer animação anterior seja parada (segurança)
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentSequence = sequence;
        currentStepIndex = 0;
        isDialogueActive = true;
        
        // CORREÇÃO: Ativa a flag para ignorar o input 'E' que está ativo neste frame.
        ignoreInitialInput = true;

        // Exibe o primeiro passo (índice 0)
        DisplayCurrentStep();

        Debug.Log("Diálogo Iniciado. Exibindo passo 0.");
    }

    /// <summary>
    /// Avança para o próximo passo na sequência.
    /// </summary>
    private void AdvanceDialogue()
    {
        // Se a animação ainda estiver rodando (teoricamente não deveria, mas é segurança)
        if (isTyping) return;

        currentStepIndex++;

        if (currentStepIndex < currentSequence.Count)
        {
            // Ainda há falas na sequência
            DisplayCurrentStep();
        }
        else
        {
            // Fim da sequência
            EndDialogue();
        }
    }

    /// <summary>
    /// Exibe o texto e ativa o painel correto para o passo atual.
    /// </summary>
    private void DisplayCurrentStep()
    {
        // Garante que ambos estejam desativados antes de ativar o correto
        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);

        DialogueStep step = currentSequence[currentStepIndex];

        if (step.type == PanelType.Dialogue)
        {
            dialoguePanel.SetActive(true);
            
            // NOVO CÓDIGO: Inicia a Coroutine de animação em vez de definir o texto diretamente.
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

    /// <summary>
    /// Coroutine para animar o texto caractere por caractere.
    /// </summary>
    private IEnumerator TypewriteText(string textToType, float delay)
    {
        isTyping = true;
        dialogueText.text = ""; // Limpa o texto

        // Itera sobre cada caractere
        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay); // Espera o tempo configurado
        }
        
        isTyping = false;
        typingCoroutine = null;
    }

    /// <summary>
    /// Interrompe a animação e exibe o texto completo imediatamente.
    /// </summary>
    private void SkipTypingAnimation()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Exibe o texto completo (já com as aspas)
        DialogueStep step = currentSequence[currentStepIndex];
        if (step.type == PanelType.Dialogue)
        {
            dialogueText.text = "\"" + step.text + "\"";
        }

        isTyping = false;
    }

    /// <summary>
    /// Encerra o diálogo, escondendo os painéis.
    /// </summary>
    private void EndDialogue()
    {
        // Garante que a coroutine pare se o diálogo terminar durante a animação
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);

        Debug.Log("Diálogo Encerrado.");
    }

    // Propriedade para que outros scripts possam verificar o estado
    public bool IsDialogueActive => isDialogueActive;
}