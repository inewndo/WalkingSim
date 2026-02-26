using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePannel;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI placeHolderOpeningLine;

    private void OnEnable()
    {
        CCPlayer.OnDialogueRequested += StartDialogue;
    }
    private void OnDisable()
    {
        CCPlayer.OnDialogueRequested -= StartDialogue;
    }

    void StartDialogue(NPCData npcData)
    {
        if(npcData == null)
        {
            Debug.Log("Data is null");
            return;
        }

        if (dialoguePannel != null) dialoguePannel.SetActive(true);
        if (displayName != null) displayName.text = npcData.displayName;
        if (placeHolderOpeningLine != null) placeHolderOpeningLine.text = npcData.placeHolderOpeningLine;
        Debug.Log($"Dialogue start with {npcData.displayName}: {npcData.placeHolderOpeningLine}");
    }

}
