using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GGJ2022.Dialogue.Schema; 

namespace GGJ2022.Dialogue
{
    public class DialogueTriggerBehavior : MonoBehaviour
    {

        // holds dialogue lines; passes them to dialogue manager when triggered

        public Cutscene dialogueToTrigger;

        public void TriggerDialogue()
        {
            Debug.Log("triggering dialogue");
            DialogueManager.instance.StartNewDialogue(dialogueToTrigger);
        }
    }
}