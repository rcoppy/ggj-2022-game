using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueRandomTriggerBehavior : MonoBehaviour {

    // holds a list of dialogues; one of which is randomly selected to display to the player

    public List<Dialogue> dialogueList = new List<Dialogue>(); 

    public void TriggerDialogue()
    {
        System.Random r = new System.Random();
        Dialogue d = dialogueList[r.Next(dialogueList.Count)];

        DialogueManager.instance.StartNewDialogue(d.lines);
    }
	
}
