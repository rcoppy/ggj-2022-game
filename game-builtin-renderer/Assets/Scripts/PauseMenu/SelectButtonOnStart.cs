using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.GetComponent<Button>().Select();
    }

    // Update is called once per frame
    public void Select()
    {
        gameObject.GetComponent<Button>().Select();
    }
}
