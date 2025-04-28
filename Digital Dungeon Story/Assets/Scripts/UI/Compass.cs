// Compass.cs
using UnityEngine;
using TMPro;

public enum CompassDirection
{
    North,
    East,
    South,
    West
}

public class Compass : MonoBehaviour
{
    public TextMeshProUGUI compassText;

    void Update()
    {
        UpdateCompass();
    }

    void UpdateCompass()
    {
        CompassDirection currentDirection = GetCurrentDirection();

        switch (currentDirection)
        {
            case CompassDirection.North:
                compassText.text = "N";
                break;
            case CompassDirection.East:
                compassText.text = "E";
                break;
            case CompassDirection.South:
                compassText.text = "S";
                break;
            case CompassDirection.West:
                compassText.text = "W";
                break;
        }
    }

    public CompassDirection GetCurrentDirection()
    {
        float yRotation = Mathf.Round(transform.eulerAngles.y);

        if ((yRotation >= 315 && yRotation <= 360) || (yRotation >= 0 && yRotation < 45))
            return CompassDirection.North;
        else if (yRotation >= 45 && yRotation < 135)
            return CompassDirection.East;
        else if (yRotation >= 135 && yRotation < 225)
            return CompassDirection.South;
        else if (yRotation >= 225 && yRotation < 315)
            return CompassDirection.West;
        else
            return CompassDirection.North; // fallback
    }
}
