using UnityEngine;
using TMPro;

public class ButtonTextToggle : MonoBehaviour
{
    private TextMeshPro buttonText;
    private bool isBlue = true;

    private void Awake()
    {
        // Specifically find the TextMeshPro component by name
        Transform targetTransform = transform.Find("CompressableButtonVisuals/IconAndText/TextMeshPro");
        if (targetTransform != null)
        {
            buttonText = targetTransform.GetComponent<TextMeshPro>();
        }

        if (buttonText != null)
        {
            buttonText.text = "Start";
        }
    }

    public void ToggleText()
    {
        if (buttonText != null)
        {
            if (isBlue)
            {
                buttonText.text = "Stop";
            }
            else
            {
                buttonText.text = "Start";
            }

            isBlue = !isBlue;
        }
    }
}
