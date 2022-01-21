using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceCamera : MonoBehaviour
{
    [SerializeField]
    Transform _targetCamera;

    Quaternion _originalLocalRotation;

    public Transform TargetCamera
    {
        set { _targetCamera = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _originalLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetCamera != null)
        {
            //var rotation = Quaternion.FromToRotation(transform.forward, _targetCamera.forward) *
            //               Quaternion.FromToRotation(transform.up, _targetCamera.up);

            //transform.rotation *= rotation

            transform.rotation = Quaternion.LookRotation(_targetCamera.forward, _targetCamera.up);


            //transform.LookAt(_targetCamera, Vector3.up);
            //transform.rotation *= _originalLocalRotation; 
        }
    }
}
