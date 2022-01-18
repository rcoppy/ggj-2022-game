using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Interactable))]
public abstract class InteractableTrigger : MonoBehaviour {

    protected Interactable interactable;
    // [SerializeField]
    // protected bool disableOnInteract = false;

    [SerializeField]
    protected string interactorTag = "Player"; // player by default; could be e.g. a bullet


    // Use this for initialization
    protected virtual void Start () {
        interactable = GetComponent<Interactable>();
	}
	
	// children need to implement interaction check in Update(), OnTriggerEnter(), etc.
    // example: 
    // update()
    //      if buttondown('a')
    //          interactable.interact();
    // 
    // point is you can swap out the trigger type (e.g. use trigger volume, button input, checking the global state for a change, etc.)
}
