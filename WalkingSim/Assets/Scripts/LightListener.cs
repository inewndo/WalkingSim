using UnityEngine;

public class LightListener : MonoBehaviour
{
    public Light sceneLight;

    private void OnEnable()
    {
        ButtonEvent.onButtonPressed += ChangeLight;
    }

    private void OnDisable()
    {
        ButtonEvent.onButtonPressed -= ChangeLight;
    }

    void ChangeLight()
    {
        sceneLight.color = Random.ColorHSV();
    }
}
