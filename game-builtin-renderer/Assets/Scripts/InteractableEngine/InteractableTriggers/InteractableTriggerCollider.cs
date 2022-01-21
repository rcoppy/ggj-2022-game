using UnityEngine;
using System.Collections;

public class InteractableTriggerCollider : InteractableTrigger
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals(interactorTag))
        {
            interactable.Interact();
        }
    }
}