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
    private bool isInteracting = false;  // Controla o estado da interação
    private bool isFirstInteraction = true; // Flag para detectar a primeira interação
    private PlayerRotation playerRotation; // Referência ao script de rotação do jogador

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCompass = player.GetComponent<Compass>();
        playerRotation = player.GetComponent<PlayerRotation>(); // Pega a referência ao script de rotação
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Verifica se o jogador está na área de interação e pressionou a tecla "E"
        if (Input.GetKeyDown(KeyCode.E) && distance <= interactionRange && playerCompass.GetCurrentDirection() == requiredDirection)
        {
            // Se não estiver interagindo, inicia a interação
            if (!isInteracting)
            {
                Interact();
            }
            else
            {
                // Se já estiver interagindo, apenas continua a interação (avança o diálogo ou descrição)
                InteractionUI.Instance.Continue();
            }
        }

        // Se o jogador sair da área ou mudar de direção, resetar a interação
        if (distance > interactionRange || playerCompass.GetCurrentDirection() != requiredDirection)
        {
            isInteracting = false; // Permitir nova interação
        }
    }

    void Interact()
    {
        // Apenas inicia a interação uma vez
        if (!isInteracting)
        {
            isInteracting = true;

            // Desabilitar rotação do jogador enquanto estiver interagindo
            playerRotation.LockRotation();

            if (isFirstInteraction) // Se for a primeira interação, inicia o diálogo imediatamente
            {
                // Marca que a primeira interação foi realizada
                isFirstInteraction = false;
            }

            switch (interactionType)
            {
                case InteractionType.Description:
                    if (descriptionLines != null && descriptionLines.Count > 0)
                    {
                        InteractionUI.Instance.StartDescription(descriptionLines, this);
                    }
                    else
                    {
                        Debug.LogWarning("Descrição vazia.");
                    }
                    break;

                case InteractionType.Dialogue:
                    if (dialogueLines != null && dialogueLines.Count > 0)
                    {
                        InteractionUI.Instance.StartDialogue(dialogueLines, this);
                    }
                    else
                    {
                        Debug.LogWarning("Diálogo vazio.");
                    }
                    break;
            }
        }
    }

    public void OnInteractionEnded()
    {
        // Resetando a interação quando ela terminar, permitindo nova interação
        isInteracting = false;

        // Habilitar rotação do jogador quando a interação terminar
        playerRotation.UnlockRotation();
    }
}
