using UnityEngine;

public class Interactable : MonoBehaviour
{
    [TextArea(3, 5)]  // Permite escrever textos maiores no Inspector
    public string interactionText = "Texto padrão"; 

    public void Interact()
    {
        Debug.Log(interactionText); // Aqui será substituído por UI depois
    }
}
