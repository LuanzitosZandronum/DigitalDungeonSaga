// Interactable.cs
using UnityEngine;
using System.Collections.Generic;
using System; // Necessário para Action

public class Interactable : MonoBehaviour
{
    // =======================================================
    // EVENTOS PARA UI (InteractionUI.cs assina estes eventos)
    // =======================================================
    // Passa o Transform do objeto interagível para que a UI saiba onde está o objeto, se necessário.
    public static event Action<Transform> OnInteractionRangeEntered;
    public static event Action OnInteractionRangeExited;

    [Header("Configuração de Interação")]
    public float interactionRange = 3f;           // Alcance máximo para interagir
    public KeyCode interactionKey = KeyCode.E;    // Chave para iniciar a interação

    [Header("Requisitos de Direção do Jogador")]
    // Se a lista estiver vazia, a direção não é um requisito.
    public List<CompassDirection> requiredDirections;

    [Header("Sequência de Diálogo/Texto")]
    public List<DialogueStep> dialogueSequence;

    // Referências (Podem ser nulas se não forem encontradas)
    private Transform playerTransform;
    private Compass playerCompass;

    // Estados de controle
    private bool isReadyToInteract = false;

    void Start()
    {
        // 1. Tenta encontrar o Player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            // 2. Tenta encontrar o Compass no Player
            playerCompass = playerObject.GetComponent<Compass>();
        }

        // VERIFICAÇÃO CRÍTICA: Se o player ou o compass não forem encontrados, desabilita o script.
        if (playerTransform == null || playerCompass == null)
        {
            Debug.LogError($"[Interactable Error] O objeto '{gameObject.name}' não conseguiu encontrar o Player (com a tag 'Player') ou o script 'Compass.cs'. Script desativado.", this);
            enabled = false;
        }
    }

    void Update()
    {
        // =======================================================
        // PARADA DE SEGURANÇA CONTRA NullReferenceException (Linhas Críticas)
        // =======================================================
        // Se qualquer dependência não foi carregada no Start, o Update para.
        if (!enabled || playerTransform == null || DialogueManager.Instance == null)
        {
            return;
        }

        // Se o DialogueManager estiver controlando um diálogo, não permita nova interação.
        if (DialogueManager.Instance.IsDialogueActive)
        {
            // Se estava pronto para interagir antes do diálogo começar, dispara o evento de saída.
            if (isReadyToInteract)
            {
                isReadyToInteract = false;
                OnInteractionRangeExited?.Invoke();
            }
            return;
        }

        // --- Lógica de Interação ---

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool canInteractNow = false;

        // 1. Verificar o Alcance
        if (distanceToPlayer <= interactionRange)
        {
            // 2. Verificar a Direção
            if (requiredDirections.Count > 0)
            {
                // **Esta linha só executa se playerCompass não for nulo (verificado no Start)**
                CompassDirection currentDir = playerCompass.GetCurrentDirection();

                if (requiredDirections.Contains(currentDir))
                {
                    canInteractNow = true;
                }
            }
            else // Não há requisitos de direção
            {
                canInteractNow = true;
            }
        }

        // =======================================================
        // LÓGICA DE EVENTOS (Controle da UI de "Pressione E")
        // =======================================================
        if (canInteractNow && !isReadyToInteract)
        {
            // Acabou de entrar no estado de interação.
            isReadyToInteract = true;
            // O '?' garante que o evento só será chamado se houver assinantes.
            OnInteractionRangeEntered?.Invoke(this.transform);
        }
        else if (!canInteractNow && isReadyToInteract)
        {
            // Acabou de sair do estado de interação.
            isReadyToInteract = false;
            OnInteractionRangeExited?.Invoke();
        }

        // 3. Iniciar Interação
        if (isReadyToInteract && Input.GetKeyDown(interactionKey))
        {
            // Inicia o diálogo
            if (dialogueSequence.Count > 0)
            {
                DialogueManager.Instance.StartDialogue(dialogueSequence);

                // Opcional: Esconder a UI de "Pressione E" imediatamente após a interação ser acionada.
                isReadyToInteract = false;
                OnInteractionRangeExited?.Invoke();
            }
            else
            {
                Debug.LogWarning($"Objeto Interagível '{gameObject.name}' foi ativado, mas não tem sequência de diálogo configurada!", this);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Desenha a esfera de alcance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}