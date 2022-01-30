using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScrollingText : MonoBehaviour
{
    RectTransform rectTrans;
    Vector3 initialPos;
    float currentPosY;
    float finalPosY;

    [SerializeField]
    float scrollSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
        rectTrans = gameObject.GetComponent<RectTransform>();
        initialPos = new Vector3(4.11f, -21.52f, 40f);
        Debug.Log("Before: " + " " + rectTrans.position);
        finalPosY = 296f;
        ResetTextPos();
        Debug.Log("After: " + " " + rectTrans.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(rectTrans.position.y > finalPosY)
        {
            ResetTextPos();
        }

        float newPosY = currentPosY + (Time.deltaTime * scrollSpeed);
        currentPosY = newPosY;
        Vector3 newPos = new Vector3(rectTrans.position.x, newPosY, rectTrans.position.z);
        rectTrans.position = newPos;
        Debug.Log("scrolling: " + " " + rectTrans.position);

        
    }

    public void ResetTextPos()
    {
        currentPosY = initialPos.y;
        rectTrans.position = initialPos;
    }
}
