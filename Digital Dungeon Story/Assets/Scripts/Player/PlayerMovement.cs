using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 1.0f;
    public float moveSpeed = 5.0f;
    private bool isMoving = false;
    private PlayerRotation playerRotation;

    public LayerMask wallLayer;
    public Vector3 collisionCheckSize = new Vector3(0.9f, 0.9f, 0.9f);

    void Start()
    {
        playerRotation = GetComponent<PlayerRotation>();
    }

    void Update()
    {
        if (!isMoving && !playerRotation.IsRotating && Input.GetKey(KeyCode.W))
        {
            if (CanMove())
            {
                StartCoroutine(MoveForward());
            }
        }
    }

    private bool CanMove()
    {
        Vector3 targetPosition = transform.position + transform.forward * moveDistance;

        Collider[] hitColliders = Physics.OverlapBox(
            center: targetPosition,
            halfExtents: collisionCheckSize / 2f,
            orientation: transform.rotation,
            layerMask: wallLayer
        );

        if (hitColliders.Length > 0)
        {
            foreach (Collider hit in hitColliders)
            {
                if (hit.transform == transform) continue;

                Collision type = hit.GetComponent<Collision>();

                if (type != null && type.collisionType == Collision.Type.Wall)
                {
                    Debug.Log("Movimento bloqueado: Colisão com Parede detectada.");
                    return false;
                }
            }
        }

        Debug.DrawRay(transform.position, transform.forward * moveDistance, Color.green, 1f);
        return true;
    }

    private System.Collections.IEnumerator MoveForward()
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + transform.forward * moveDistance;

        float elapsedTime = 0;
        float moveDuration = moveDistance / moveSpeed;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXByName("Footstep");
        }

        isMoving = false;
    }
}