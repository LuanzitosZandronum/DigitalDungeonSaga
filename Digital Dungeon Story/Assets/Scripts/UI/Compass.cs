using UnityEngine;
using TMPro;


public enum CompassDirection
{
    North,
    South,
    East,
    West
}

public class Compass : MonoBehaviour
{
    public TextMeshProUGUI compassText;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

public CompassDirection GetCurrentDirection()
{
    float y = transform.eulerAngles.y;

    if (y >= 315 || y < 45)
        return CompassDirection.North;
    else if (y >= 45 && y < 135)
        return CompassDirection.East;
    else if (y >= 135 && y < 225)
        return CompassDirection.South;
    else
        return CompassDirection.West;
}

    void Update()
    {
        float y = player.eulerAngles.y;

        // Arredonda para o múltiplo de 90° mais próximo
        int direction = Mathf.RoundToInt(y / 90f) * 90 % 360;

        switch (direction)
        {
            case 0:
                compassText.text = "N";
                break;
            case 90:
                compassText.text = "E"; // ou "E"
                break;
            case 180:
                compassText.text = "S";
                break;
            case 270:
                compassText.text = "W"; // ou "W"
                break;
        }
    }
}
