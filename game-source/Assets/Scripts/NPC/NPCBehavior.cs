using UnityEngine;
using System.Collections;

public class NPCBehavior : MonoBehaviour {

    public string lookTargetTag = "Player";
    public float lerpFactor = 0.001f;

    private bool isTargetInRange = false;
    private Transform target = null;
    private Vector3 startRot;

	// Use this for initialization
	void Start () {
        startRot = transform.localRotation.eulerAngles; 
	}
	
	// Update is called once per frame
	void Update () {
        if (isTargetInRange)
        {
            if (target != null) {
                // rotate to face target
                Quaternion rot = transform.localRotation;
                rot.SetLookRotation(target.position - transform.position);
                
                rot = Quaternion.Euler(startRot.x, rot.eulerAngles.y, startRot.y);
                // print(rot.eulerAngles.y);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, lerpFactor);
                // print("rotated to face target");
            }
        }	
	}

    public bool GetIsTargetInRange()
    {
        return isTargetInRange; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(lookTargetTag))
        {
            isTargetInRange = true;
            target = other.gameObject.transform;
            // print(other.gameObject.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals(lookTargetTag))
        {
            isTargetInRange = false;
        }
    }
}
