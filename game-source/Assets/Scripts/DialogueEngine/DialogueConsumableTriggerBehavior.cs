using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueConsumableTriggerBehavior : MonoBehaviour {
    // takes two dialogues; plays the first only once, then the second all subsequent times

    public Dialogue consumableDialogue;
    public Dialogue finalDialogue;

    public bool isConsumed = false; 

    public void TriggerDialogue()
    {
        Dialogue d = finalDialogue;
        
        if (!isConsumed)
        {
            d = consumableDialogue;
            isConsumed = true; 
        }

        DialogueManager.instance.StartNewDialogue(d.lines); 

    }
}
