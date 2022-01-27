using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectThisButton : MonoBehaviour
{
  
    public void Select()
    {
        gameObject.GetComponent<Button>().Select();
    }
}
