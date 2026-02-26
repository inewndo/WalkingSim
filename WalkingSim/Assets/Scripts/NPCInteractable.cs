using UnityEngine;

public class NPCInteractable : Interactable
{
    public NPCData npcData;

    public override void Interact(CCPlayer ccplayer)
    {
        if(npcData == null)
        {
            Debug.Log("npc no data" + gameObject.name);
        }

        //when interact w npc, request dialogue data
        ccplayer.RequestDialogue(npcData);
    }
}
