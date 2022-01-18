using UnityEngine;
using System.Collections;

public class InteractableTriggerButton : InteractableTrigger
{
    // parent gameobject needs to have a trigger collider

    [SerializeField]
    private string myButton = "Fire1";

    private void OnTriggerStay(Collider other)
    {
	    if (other.gameObject.tag.Equals(interactorTag) && Input.GetButtonDown(myButton))
        {
            interactable.Interact();

            /*if (disableOnInteract)
            {
                interactable.canInteract = false;
            }*/
        }
	}
}
