using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public Sprite portrait;
}

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    public GameObject dialoguePanel;
    public Image dialogueImage;
    public TextMeshProUGUI dialogueText;

    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;

    private List<string> currentDescriptionLines;
    private int currentDescriptionIndex;

    private List<DialogueLine> currentDialogueLines;
    private int currentDialogueIndex;

    public bool isInteracting = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDescription(List<string> lines)
    {
        isInteracting = true;
        descriptionPanel.SetActive(true);
        currentDescriptionLines = lines;
        currentDescriptionIndex = 0;
        ShowNextDescription();
        LockPlayerMovement();
    }

    void ShowNextDescription()
    {
        if (currentDescriptionIndex >= currentDescriptionLines.Count)
        {
            EndInteraction();
            return;
        }

        descriptionText.text = currentDescriptionLines[currentDescriptionIndex];
        currentDescriptionIndex++;
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        isInteracting = true;
        dialoguePanel.SetActive(true);
        currentDialogueLines = lines;
        currentDialogueIndex = 0;
        ShowCurrentDialogue();
        LockPlayerMovement();
    }

    void ShowCurrentDialogue()
    {
        if (currentDialogueLines == null || currentDialogueIndex >= currentDialogueLines.Count)
        {
            EndInteraction();
            return;
        }

        DialogueLine line = currentDialogueLines[currentDialogueIndex];

        if (dialogueImage != null && line.portrait != null)
        {
            dialogueImage.sprite = line.portrait;
        }

        dialogueText.text = line.text;
    }

    public void Continue()
    {
        if (!isInteracting) return;

        if (descriptionPanel.activeSelf)
        {
            ShowNextDescription();
        }
        else if (dialoguePanel.activeSelf)
        {
            currentDialogueIndex++; // <--- Avança o índice aqui
            ShowCurrentDialogue();
        }
    }

    public void EndInteraction()
    {
        isInteracting = false;
        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);
        UnlockPlayerMovement();
    }

    void LockPlayerMovement()
    {
        PlayerMovement movement = FindObjectOfType<PlayerMovement>();
        if (movement != null) movement.enabled = false;
    }

    void UnlockPlayerMovement()
    {
        PlayerMovement movement = FindObjectOfType<PlayerMovement>();
        if (movement != null) movement.enabled = true;
    }
}
