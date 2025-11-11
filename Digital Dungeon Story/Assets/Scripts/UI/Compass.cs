using UnityEngine;
using TMPro;

// Definindo o enum CompassDirection UMA ÚNICA VEZ aqui.
public enum CompassDirection
{
    North,
    South,
    East,
    West,
    None // Adicionado para cobrir casos onde a direção não é relevante (opcional)
}

public class Compass : MonoBehaviour
{
    public TextMeshProUGUI compassText;
    private Transform player;

    void Start()
    {
        // Garante que o Player seja encontrado
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Objeto com a tag 'Player' não encontrado. O Compass não funcionará.");
            enabled = false; // Desativa o script se o Player não for encontrado
        }
    }

    /// <summary>
    /// Retorna a direção cardeal baseada na rotação atual do Player.
    /// </summary>
    public CompassDirection GetCurrentDirection()
    {
        if (player == null) return CompassDirection.None;

        // CORREÇÃO AQUI: Usar a rotação do Player (player.eulerAngles.y)
        float y = player.eulerAngles.y;

        // Normaliza o ângulo para 0-360
        y = y % 360;

        // Note: A lógica dos quadrantes está correta para eixos de 45 graus.
        if (y >= 315 || y < 45)
            return CompassDirection.North; // 315° a 45°
        else if (y >= 45 && y < 135)
            return CompassDirection.East; // 45° a 135°
        else if (y >= 135 && y < 225)
            return CompassDirection.South; // 135° a 225°
        else // y >= 225 && y < 315
            return CompassDirection.West; // 225° a 315°
    }

    void Update()
    {
        if (player == null) return;

        float y = player.eulerAngles.y;

        // Calcula a direção cardeal para exibição (arredondando para o múltiplo de 90°)
        int direction = Mathf.RoundToInt(y / 90f) * 90 % 360;

        // Corrige o 360 para 0 (se necessário)
        if (direction == 360) direction = 0;

        string directionText = "";

        switch (direction)
        {
            case 0:
                directionText = "N";
                break;
            case 90:
                directionText = "E";
                break;
            case 180:
                directionText = "S";
                break;
            case 270:
                directionText = "W";
                break;
        }

        compassText.text = directionText;
    }
}