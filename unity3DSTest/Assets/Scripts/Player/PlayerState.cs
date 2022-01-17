using UnityEngine;
using System.Collections;
using ECM.Controllers; 

public class PlayerState : MonoBehaviour {

    #region component vars
    private bool areAllComponentsFrozen = false;
    private bool isInputFrozen = false;
    private bool isAnimationFrozen = false;
    private bool areStateVarsFrozen = false;
    private bool isPhysicsFrozen = false;

    private BaseCharacterController input;
    private Animator animator;
    public FollowCamPositioning followCam;

    #endregion 

    #region getters
    public bool GetAreAllComponentsFrozen()
    {
        return areAllComponentsFrozen;
    }

    public bool GetIsInputFrozen()
    {
        return isInputFrozen;
    }

    public bool GetIsAnimationFrozen()
    {
        return isAnimationFrozen;
    }

    public bool GetAreStateVarsFrozen()
    {
        return areStateVarsFrozen;
    }

    public bool GetIsPhysicsFrozen()
    {
        return isPhysicsFrozen;
    }
    #endregion

    public static PlayerState instance = null;

    private void Awake()
    {
        #region singleton
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a CameraManager.
            Destroy(gameObject);

        // let this object be destroyed on scene reload 
        #endregion
    }
    // Use this for initialization
    void Start () {
        input = GetComponent<BaseCharacterController>();
        animator = GetComponent<Animator>();
    }

    #region freeze/unfreeze methods
    public void FreezeInput()
    {
        // input.enabled = false;
        // followCam.isFrozen = true; 
        isInputFrozen = true; 
    }

    public void UnfreezeInput()
    {
        // input.enabled = true;
        // followCam.isFrozen = false;
        isInputFrozen = false; 
    }

    public void FreezeAnimation()
    {
        isAnimationFrozen = true; 
    }

    public void UnFreezeAnimation()
    {
        isAnimationFrozen = false; 
    }
    #endregion

}
