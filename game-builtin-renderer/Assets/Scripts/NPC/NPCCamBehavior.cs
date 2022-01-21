using UnityEngine;
using System.Collections;

public class NPCCamBehavior : MonoBehaviour {

    
    public Camera myCam;
    public float transitionTime = 0.5f;

    NPCBehavior behavior;
    CameraManager camManager;

	// Use this for initialization
	void Start () {
        behavior = GetComponent<NPCBehavior>();
        camManager = CameraManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
	    if (behavior.GetIsTargetInRange())
        {
            if (DialogueManager.instance.GetIsDialogueActive())
            {
                if (camManager.GetActiveCam() != myCam)
                {
                    camManager.SwitchToCam(myCam, transitionTime);
                    // PlayerState.instance.FreezeInput();

                }
            }
            else if (camManager.GetActiveCam() == myCam)
            {
                camManager.SwitchToCam(camManager.startCam, transitionTime);
                // PlayerState.instance.UnfreezeInput();
            }
        }
        else
        {
            if (camManager.GetActiveCam() == myCam)
            {
                camManager.SwitchToCam(camManager.startCam, transitionTime);
                // PlayerState.instance.UnfreezeInput();
            }
        }
	}
}
