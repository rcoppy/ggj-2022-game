using UnityEngine;
using System.Collections;

public class InteractableTriggerVolume : InteractableTrigger {

    // collider needs to be attached to the parent gameobject
    
    [SerializeField]
    [Tooltip("True, use OnTriggerEnter; false use OnTriggerExit")]
    private bool isOnEnter = true; // if false, uses OnTriggerExit

    /*protected override void Start()
    {
        base.Start();
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(interactorTag) && isOnEnter)
        {
            interactable.Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals(interactorTag) && !isOnEnter)
        {
            interactable.Interact();
        }
    }
}
