using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// see these tutorials:
// https://www.youtube.com/watch?v=mXjRR1nnC5M
// https://www.youtube.com/watch?v=_nRzoTzeyxU

public class DialogueManager : MonoBehaviour {

    // singleton pattern
    public static DialogueManager instance { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            instance = this; 
        }
    }

    private Queue<string> dialogueLines;
    public GameObject DialogueCanvas;
    private Text textObject;
    private bool isDialogueActive = false;
    public string continueButton = "Fire1"; 

    public bool GetIsDialogueActive()
    {
        return isDialogueActive;
    }

    private void Start()
    {
        textObject = DialogueCanvas.GetComponentInChildren<Text>();
        DialogueCanvas.SetActive(false);
    }

    public void StartNewDialogue (string[] lines)
    {
        // queue: first-in, first-out
        // first line of dialogue to be added will be first line to be returned
        isDialogueActive = true; 

        dialogueLines = new Queue<string>(lines);

        DialogueCanvas.SetActive(true);

        UpdateDialogue(); // step through one line

        PlayerState.instance.FreezeInput();
    }

    public void UpdateDialogue()
    {
        if (dialogueLines.Count > 0)
        {
            string line = dialogueLines.Dequeue(); // will return and remove oldest (first added) element

            textObject.text = line;

            Debug.Log(line); 
        }
        else 
        {
            // close the dialogue
            DialogueCanvas.SetActive(false);
            isDialogueActive = false;

            // assume dialogue was triggered by an interaction; end that interaction
            //if (Interactable.isInteractionInProgress)
            //{
            Interactable.EndInteraction();
            Debug.Log("isinteract false"); 
            //} // end that interaction (hackey; not ideal; fix this)

            PlayerState.instance.UnfreezeInput();
        }
    }

    // when fire1 button is pressed, continue the active dialogue 

    bool waitFlag = false; // need to wait for a frame, otherwise first speech line gets skipped 

    void Update()
    {
        if (DialogueManager.instance.GetIsDialogueActive())
        {
            if (waitFlag)
            {
                if (Input.GetButtonDown(continueButton))
                {
                    DialogueManager.instance.UpdateDialogue();
                }
            }
            else
            {
                waitFlag = true;
            }
        }
        else
        {
            waitFlag = false; // reset flag
        }
    }
}
