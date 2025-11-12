using UnityEngine;
using TMPro;

public enum CompassDirection
{
    North,
    South,
    East,
    West,
    None
}

public class Compass : MonoBehaviour
{
    public TextMeshProUGUI compassText;
    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Objeto com a tag 'Player' não encontrado. O Compass não funcionará.");
            enabled = false;
        }
    }

    public CompassDirection GetCurrentDirection()
    {
        if (player == null) return CompassDirection.None;

        float y = player.eulerAngles.y;

        y = y % 360;

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
        if (player == null) return;

        float y = player.eulerAngles.y;

        int direction = Mathf.RoundToInt(y / 90f) * 90 % 360;

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