using System;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    //action is a delegate- a variable that can store a function
    public static event Action onButtonPressed;


    public void OnButtonPressed()
    {
        onButtonPressed?.Invoke();
    }
    

}
