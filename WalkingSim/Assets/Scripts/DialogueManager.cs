using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePannel;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI lineText;
    public Transform choicesContainer; //parent object where choice buttons will spawn
    public Button choiceButtonPrefab; //single choice button

    private int lineindex; //line index whichx we are currently on in the dialogue
    private bool isActive; //track if we are currently in dialogue
    private NPCData currentNode;
    //lock player movemnt and cursor when in dialogue
    private CCPlayer player;

    private void Awake()
    {
        //start w dialogue hidden
        if (dialoguePannel != null) dialoguePannel.SetActive(false);
        ClearChoices();
        player = FindFirstObjectByType<CCPlayer>();
    }
    private void OnEnable()
    {
        CCPlayer.OnDialogueRequested += StartDialogue;
    }
    private void OnDisable()
    {
        CCPlayer.OnDialogueRequested -= StartDialogue;
    }

    private void Update()
    {
        if (!isActive) return; //if no dialogue active then return
        if(Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (ChoicesAreShowing()) return;
            Advanced();
        }

        //keyboard.current.qKey
    }
    void StartDialogue(NPCData npcData)
    {
        if(npcData == null)
        {
            Debug.Log("Data is null");
            return;
        }

        currentNode = npcData;
        lineindex = 0;
        isActive = true;

        if (dialoguePannel != null) dialoguePannel.SetActive(true);
        ShowLine();
    }

    bool HasChoices(NPCData node)
    {
        return node != null && node.choices != null && node.choices.Length > 0;
    }

    void Advanced()
    {
        //if node is finished and dialogue
        if(currentNode == null)
        {
            EndDialogue();
            return;
        }
        //move to next line
        lineindex++;

        //if there are still line to read in the node then show them
        if(currentNode.lines != null && lineindex < currentNode.lines.Length)
        {
            //if we have smth
            if(lineText != null)
            {
                //take the text of outTMP obj and change it to wtv the current line is
                lineText.text = currentNode.lines[lineindex];
                return;
            }
        }
        //otherwise we have reached the end
        FinishNode();
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        ClearChoices();
        if(choicesContainer == null || choiceButtonPrefab == null)
        {
            Debug.Log("choices are not wired");
            return;
        }

        foreach(DialogueChoice choice in choices)
        {
            Button bttn = Instantiate(choiceButtonPrefab, choicesContainer);

            Debug.Log("ButtonSpawm");

            TextMeshProUGUI tmp = bttn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = choice.choiceText;

            //cache next node in a local var
            NPCData next = choice.nextNode;

            //lambda
            bttn.onClick.AddListener(() =>
            {
                Choose(next);
            });
        }
    }

    void FinishNode()
    {
        //if choice exist then show, else if next node exists- continue automotaicall, else end dialogue
        if (HasChoices(currentNode))
        {
            ShowChoices(currentNode.choices);
            Debug.Log("FinishNode");
            return;
        }
        //auto continue text
        if(currentNode.nextNode != null)
        {
            currentNode = currentNode.nextNode;
            lineindex = 0;
            return;
        }
        EndDialogue();
    }

    void ShowLine()
    {
        //when showing a  line we should not be showing choices
        ClearChoices();

        //if no node-end dialogue
        if(currentNode == null)
        {
            EndDialogue();
            return;
        }

        //update speaker name
        if (displayName != null) displayName.text = currentNode.displayName;
        if (currentNode.lines == null || currentNode.lines.Length == 0)
        {
            FinishNode();
            return;
        }
        lineindex = Mathf.Clamp(lineindex, 0, currentNode.lines.Length - 1);
        if (lineText != null) lineText.text = currentNode.lines[lineindex];
    }

    void Choose(NPCData nextNode)
    {
        //remove buttons asap so ui feels responsive
        ClearChoices();

        //if no n ext node this ends convo
        if(nextNode == null)
        {
            EndDialogue();
            return;
        }
        //otherwise go to chosen node
        currentNode = nextNode;
        lineindex = 0;
        ShowLine();
    }

    bool ChoicesAreShowing()
    {
        return choicesContainer != null && choicesContainer.childCount > 0;

        //bool showing = choicesContainer != null && choicesContainer.childCount > 0;
        //Debug.Log(showing);
        //return;
    }

    void ClearChoices()
    {
        //if no choice container then exit the function
        if (choicesContainer == null) return;

        for (int i = choicesContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(choicesContainer.GetChild(i).gameObject);
        }
    }

    void EndDialogue()
    {
        //lock player cam

        isActive = false; //no longer in dialogue
        currentNode = null;
        lineindex = 0;

        ClearChoices();

        //turn off dialogue panne;
        if (dialoguePannel != null) dialoguePannel.SetActive(false);
    }
}
