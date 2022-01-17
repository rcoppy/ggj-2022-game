using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRelativeController : ECM.Controllers.BaseCharacterController
{
    // [Header("A CameraManager needs to exist in the scene for this script to work correctly.")]
    // public Camera cam; 

    protected override void HandleInput()
    {
        if (!PlayerState.instance.GetIsInputFrozen())
        {
#if UNITY_N3DS && !UNITY_EDITOR
            // Nintendo 3DS-specific code here
            Vector2 circlePad = UnityEngine.N3DS.GamePad.CirclePad;
            moveDirection = new Vector3
            {
                x = circlePad.x,
                y = 0.0f,
                z = circlePad.y
            };

#else
            moveDirection = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0.0f,
                z = Input.GetAxisRaw("Vertical")
            };
#endif

       

            // compensate for pitch/roll of camera 
            Camera cam = CameraManager.instance.GetActiveCam();
            Vector3 compForward = cam.transform.forward;
            Vector3 compRight = cam.transform.right;

            compForward.y = 0;
            compRight.y = 0;

            compForward.Normalize();
            compRight.Normalize();

            moveDirection = moveDirection.z * compForward + moveDirection.x * compRight;


            jump = Input.GetButton("Jump");
        }
        else
        {
            moveDirection = Vector3.zero;
            jump = false; 
        }
    }
}
