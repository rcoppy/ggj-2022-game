using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable() 
    {
        gameObject.GetComponent<Button>().Select();
    }


}
