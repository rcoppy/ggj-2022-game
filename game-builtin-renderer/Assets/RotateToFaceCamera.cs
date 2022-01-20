using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RotateToFaceCamera : MonoBehaviour
{
    [SerializeField]
    Transform _targetCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetCamera != null)
        {
            var rotation = Quaternion.FromToRotation(transform.forward, _targetCamera.forward) *
                           Quaternion.FromToRotation(transform.up, _targetCamera.up);

            transform.rotation = rotation; 
        }
    }
}
