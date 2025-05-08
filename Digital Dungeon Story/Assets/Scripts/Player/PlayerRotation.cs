using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float rotationSpeed = 300f; // Velocidade da rotação
    private bool isRotating = false;
    private bool isRotationLocked = false;  // Flag para bloquear rotação durante interação
    public bool IsRotating => isRotating; // Permite que outros scripts verifiquem se o jogador está girando

    void Update()
    {
        if (!isRotating && !isRotationLocked) // Impede rotação se estiver bloqueado
        {
            if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(RotatePlayer(-90));
            else if (Input.GetKeyDown(KeyCode.D))
                StartCoroutine(RotatePlayer(90));
            else if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(RotatePlayer(180));
        }
    }

    private System.Collections.IEnumerator RotatePlayer(float angle)
    {
        isRotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, angle, 0);
        
        float elapsedTime = 0;
        while (elapsedTime < Mathf.Abs(angle) / rotationSpeed)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / (Mathf.Abs(angle) / rotationSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }

    // Método para bloquear a rotação
    public void LockRotation()
    {
        isRotationLocked = true;
    }

    // Método para desbloquear a rotação
    public void UnlockRotation()
    {
        isRotationLocked = false;
    }
}
