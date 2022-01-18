using UnityEngine;
using System.Collections;
using UnityEngine.Events; 

public class NPCTriggerDialogue : MonoBehaviour {

    // activate NPC's dialogue lines if
    // --player is in range
    // --no dialogue is active
    // this code runs independently from the cam switch that happens if player is in range
    // (since the cam switch will probably be removed/redesigned)

    NPCBehavior behavior;
    DialogueTriggerBehavior trigger; 

    // Use this for initialization
    void Start () {
        behavior = GetComponent<NPCBehavior>();
        trigger = GetComponent<DialogueTriggerBehavior>();
	}

    // Update is called once per frame
    void Update()
    {
        if (behavior.GetIsTargetInRange())
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!DialogueManager.instance.GetIsDialogueActive())
                {
                    trigger.TriggerDialogue();
                }
            }
        }
    }
}
