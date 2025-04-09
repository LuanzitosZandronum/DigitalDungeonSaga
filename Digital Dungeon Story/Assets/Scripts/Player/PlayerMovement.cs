using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1.0f; // Distância de cada movimento no grid
    public float moveSpeed = 5.0f; // Velocidade do movimento
    private bool isMoving = false;
    
    private PlayerRotation playerRotation; // Referência ao script de rotação

    void Start()
    {
        playerRotation = GetComponent<PlayerRotation>();
    }

    void Update()
    {
        if (!isMoving && !playerRotation.IsRotating && Input.GetKey(KeyCode.W))
        {
            StartCoroutine(MoveForward());
        }
    }

    private System.Collections.IEnumerator MoveForward()
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + transform.forward * moveDistance;

        float elapsedTime = 0;
        while (elapsedTime < moveDistance / moveSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / (moveDistance / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}
