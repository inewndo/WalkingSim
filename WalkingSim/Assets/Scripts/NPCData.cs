using UnityEngine;
[CreateAssetMenu(menuName = "Dialogue/NPC Data")]

public class NPCData : ScriptableObject
{
    [Header("Speaker")]
    public string displayName;

    [Header("Dialogue")]
    [TextArea(3, 10)]
    public string[] lines;

    [Header("if there are no choices, we show buttons after lines end")]
    public DialogueChoice[] choices;

    [Header("if there are no choices, auto continue to next node")]
    public NPCData nextNode;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public NPCData nextNode;
}
