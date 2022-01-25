using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceCamera : MonoBehaviour
{
    [SerializeField]
    Transform _targetCamera;

    [SerializeField]
    Vector3 _baseEulerRotation; 

    public Transform TargetCamera
    {
        set { _targetCamera = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // _originalLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetCamera != null)
        {
            //var rotation = Quaternion.FromToRotation(transform.forward, _targetCamera.forward) *
            //               Quaternion.FromToRotation(transform.up, _targetCamera.up);

            //transform.rotation *= rotation

            var intermediate = Quaternion.LookRotation(_targetCamera.forward, _targetCamera.up);

            // preserve the up axis
            var correction = Quaternion.FromToRotation(intermediate * Vector3.up, Vector3.up);

            transform.rotation = Quaternion.Euler(_baseEulerRotation) * correction; 


            //transform.LookAt(_targetCamera, Vector3.up);
            //transform.rotation *= _originalLocalRotation; 
        }
    }
}
