using UnityEngine;
using UnityEngine.UI;

public class ButtonToggler : MonoBehaviour
{
    public Button button;

    public void ToggleButton()
    {
        // toggle the button image
        button.targetGraphic.enabled = !button.targetGraphic.enabled;
        // toggle the button's click functionality
        button.interactable = !button.interactable;
    }
}