using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Compass : MonoBehaviour
{
    public TextMeshProUGUI compassText; // Referência ao texto na UI

    void Update()
    {
        UpdateCompass();
    }

    void UpdateCompass()
    {
        float yRotation = Mathf.Round(transform.eulerAngles.y); // Arredonda para evitar imprecisões

        switch ((int)yRotation)
        {
            case 0:
            case 360:
                compassText.text = "N";
                break;
            case 90:
                compassText.text = "E";
                break;
            case 180:
                compassText.text = "S";
                break;
            case 270:
                compassText.text = "W";
                break;
        }
    }
}
