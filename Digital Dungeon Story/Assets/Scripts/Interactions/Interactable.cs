using UnityEngine;

public enum InteractionType
{
    Description,
    Dialogue
}

public class InteractableObject : MonoBehaviour
{
    public InteractionType interactionType;
    public string textToShow;
    public Sprite characterImage;
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

        if (distance <= interactionRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (playerCompass.GetCurrentDirection() == requiredDirection)
                {
                    Interact();
                }
            }
        }
    }

    void Interact()
    {
        Debug.Log("Interagiu com " + gameObject.name);
    }
}
