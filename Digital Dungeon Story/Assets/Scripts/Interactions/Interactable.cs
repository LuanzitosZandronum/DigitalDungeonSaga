using UnityEngine;
using System.Collections.Generic;
using System;

public class Interactable : MonoBehaviour
{
    public static event Action<Transform> OnInteractionRangeEntered;
    public static event Action OnInteractionRangeExited;

    [Header("Configuração de Interação")]
    public float interactionRange = 3f;
    public KeyCode interactionKey = KeyCode.E;

    [Tooltip("Se marcado, o diálogo começa assim que o jogador entra no range (e cumpre os requisitos de direção), sem precisar apertar a tecla.")]
    public bool triggerOnEnter = false;

    [Header("Requisitos de Direção do Jogador")]
    public List<CompassDirection> requiredDirections;

    [Header("Sequência de Diálogo/Texto")]
    public List<DialogueStep> dialogueSequence;

    private Transform playerTransform;
    private Compass playerCompass;
    private bool isReadyToInteract = false;

    private bool hasBeenTriggeredAuto = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerCompass = playerObject.GetComponent<Compass>();
        }
        if (playerTransform == null || playerCompass == null)
        {
            Debug.LogError($"[Interactable Error] O objeto '{gameObject.name}' não conseguiu encontrar o Player (com a tag 'Player') ou o script 'Compass.cs'. Script desativado.", this);
            enabled = false;
        }
    }

    private void ActivateInteraction()
    {
        if (dialogueSequence.Count > 0)
        {
            DialogueManager.Instance.StartDialogue(dialogueSequence);

            OnInteractionRangeExited?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Objeto Interagível '{gameObject.name}' foi ativado, mas não tem sequência de diálogo configurada!", this);
        }
    }

    void Update()
    {
        if (!enabled || playerTransform == null || DialogueManager.Instance == null) return;

        if (triggerOnEnter && hasBeenTriggeredAuto) return;

        if (DialogueManager.Instance.IsDialogueActive)
        {
            if (isReadyToInteract)
            {
                isReadyToInteract = false;
                OnInteractionRangeExited?.Invoke();
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool canInteractNow = false;

        if (distanceToPlayer <= interactionRange)
        {
            if (requiredDirections.Count > 0)
            {
                CompassDirection currentDir = playerCompass.GetCurrentDirection();
                if (requiredDirections.Contains(currentDir))
                {
                    canInteractNow = true;
                }
            }
            else
            {
                canInteractNow = true;
            }
        }


        if (triggerOnEnter)
        {

            if (canInteractNow)
            {
                hasBeenTriggeredAuto = true;
                ActivateInteraction();
            }
        }
        else
        {

            if (canInteractNow && !isReadyToInteract && !Input.GetKeyDown(interactionKey))
            {
                isReadyToInteract = true;
                OnInteractionRangeEntered?.Invoke(this.transform);
            }
            else if (!canInteractNow && isReadyToInteract)
            {
                isReadyToInteract = false;
                OnInteractionRangeExited?.Invoke();
            }

            if (isReadyToInteract && Input.GetKeyDown(interactionKey))
            {
                ActivateInteraction();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}