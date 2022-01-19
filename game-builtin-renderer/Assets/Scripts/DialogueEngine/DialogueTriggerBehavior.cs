using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueTriggerBehavior : MonoBehaviour {

    // holds dialogue lines; passes them to dialogue manager when triggered

    public Dialogue dialogueToTrigger;

    public void TriggerDialogue()
    {
        Debug.Log("triggering dialogue");
        DialogueManager.instance.StartNewDialogue(dialogueToTrigger.lines);
    }
}
