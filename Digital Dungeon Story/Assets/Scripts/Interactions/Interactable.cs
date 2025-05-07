using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum InteractionType { Description, Dialogue }

    public InteractionType interactionType;
    public List<string> descriptionLines;
    public List<DialogueLine> dialogueLines;

    public CompassDirection requiredDirection;
    public float interactionRange = 3f;

    private Transform player;
    private Compass playerCompass;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCompass = player.GetComponent<Compass>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (InteractionUI.Instance != null && InteractionUI.Instance.isInteracting)
            {
                InteractionUI.Instance.Continue();
                return;
            }

            if (distance <= interactionRange && playerCompass.GetCurrentDirection() == requiredDirection)
            {
                Interact();
            }
        }
    }

    void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.Description:
                if (descriptionLines != null && descriptionLines.Count > 0)
                {
                    InteractionUI.Instance.StartDescription(descriptionLines);
                }
                else
                {
                    Debug.LogWarning("Descrição vazia.");
                }
                break;

            case InteractionType.Dialogue:
                if (dialogueLines != null && dialogueLines.Count > 0)
                {
                    InteractionUI.Instance.StartDialogue(dialogueLines);
                }
                else
                {
                    Debug.LogWarning("Diálogo vazio.");
                }
                break;
        }
    }

}
