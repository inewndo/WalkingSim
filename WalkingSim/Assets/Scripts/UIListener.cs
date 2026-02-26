using TMPro;
using UnityEngine;

public class UIListener : MonoBehaviour
{
    public TextMeshProUGUI statusText;


    private void OnEnable()
    {
        ButtonEvent.onButtonPressed += UpdateText;
    }
    private void OnDisable()
    {
        ButtonEvent.onButtonPressed -= UpdateText;
    }
    void UpdateText()
    {
        statusText.text = "Button Pressed";
    }
}
