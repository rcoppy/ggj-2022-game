using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueConditionalTriggerBehavior : MonoBehaviour {

    // holds three dialogue lines; passes one when a condition is met, the other when it isn't
    // the third is used optionally 

    public Dialogue positiveDialogue; // if condition is met (e.g. 'you brought me the thing!')
    public Dialogue negativeDialogue; // if condition isn't met (e.g. 'please bring me the thing!')
    public Dialogue conditionWasMetDialogue; // if the condition was met, but no longer is (e.g. 'thanks for bringing me the thing earlier!')
    
    public bool useConditionWasMet = false; 

    [SerializeField]
    private bool isConditionMet = false;
    private bool wasConditionMet = false;
    

    public void TriggerDialogue()
    {
        // Debug.Log("triggering dialogue");
        Dialogue d;

        if (isConditionMet)
        {
            d = positiveDialogue;
        }
        else
        {
            if (useConditionWasMet && wasConditionMet)
            {
                d = conditionWasMetDialogue;
            }
            else
            {
                d = negativeDialogue; 
            }
        }

        DialogueManager.instance.StartNewDialogue(d.lines);
    }

    public void SetIsConditionMet(bool c)
    {
        isConditionMet = c; 
        
        if (c)
        {
            wasConditionMet = true; 
        }
    }

    public bool GetIsConditionMet()
    {
        return isConditionMet; 
    }


}
