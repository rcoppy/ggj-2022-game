using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {

    // on interact, invoke editor-assigned callback (e.g. start dialogue)
    // should live on same gameobject as respective InteractableTrigger (the object that will fire 'interact')
    // why different trigger components? 
    // because the way you trigger the callback should be arbitrary (could be a button press, a collision, a state change)

    public UnityEvent callback;
    public UnityEvent endCallback; // editor-assigned callback invoked on end of interaction
    public bool canInteract = true; // is interaction enabled or disabled

    // static vars
    private static bool isInteractionInProgress = false; // if an interaction is already happening, don't let this one start/restart
    private static UnityEvent endStaticCallback = null; // global end callback 

    [SerializeField]
    private bool isConsumable = false; // a one-time-use interaction?
    [SerializeField]
    private bool isConsumed = false; // has been used yet?


    public bool GetIsConsumed()
    {
        return isConsumed; 
    }

    public static void EndInteraction()
    {
        isInteractionInProgress = false;
        if (endStaticCallback != null)
        {
            endStaticCallback.Invoke();
            endStaticCallback = null; // consume the callback 
        }
    }

    // optionally pass a function to be called on the interaction's completion 
    /*public bool Interact(UnityEvent endCallback)
    {
        bool flag = Interact();
        if (flag)
        {
            endStaticCallback = endCallback; 
        }
        return flag; 
    }*/

    public bool Interact()
    {
        bool flag = false; // did the interact pass
        Debug.Log("attempting interact"); 
        if (canInteract && !isInteractionInProgress)
        {
            if (isConsumable)
            {
                if (!isConsumed)
                {
                    isConsumed = true;
                    flag = true;  
                }
            } else
            {
                flag = true; 
            }
        } else
        {
            Debug.Log("interact failed, isint: " + isInteractionInProgress); 
        }

        if (flag) 
        {
            Interactable.isInteractionInProgress = true; // callback needs to set this back to false
            Interactable.endStaticCallback = endCallback; 
            callback.Invoke();
            Debug.Log("interact succeeded");
        }

        return flag; 
    }
}
