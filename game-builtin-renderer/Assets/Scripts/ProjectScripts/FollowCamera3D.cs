using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCoords))]
public class FollowCamera3D : MonoBehaviour
{

    [SerializeField]
    Transform _target;

    [SerializeField]
    float _angularLerpFactor = 0.08f; // degrees

    
    [SerializeField]
    Vector3 _safeZoneExtent;

    [SerializeField]
    Vector3 _safezoneOffset;


    [SerializeField]
    Vector3 _outerZoneExtent;

    [SerializeField]
    Vector3 _outerZoneOffset;


    bool _moving = false;

    Vector3 _velocity;
    // Quaternion _angularVelocity; 

    [SerializeField]
    float _friction = 0.1f;

    [SerializeField]
    float _acceleration = 12f;

    [SerializeField]
    float _catchupDampingFactor = 3.5f; 

    [SerializeField]
    float _maxSpeed = 20f;

    [SerializeField]
    bool _killAcceleration = true;

    [SerializeField]
    bool _scaleBoundsWithAspect = true;

    float _aspectScaling = 1f;

    float _baseAspectScaling = 1f;

    float _baseAspectRatio = 16f / 9f;

    Vector3 _originPosition; 

    //[SerializeField]
    //AdaptiveAspectRatio _aspectTracker;

    //AdaptCameraSizeToAspect _aspectAdapter;

    Vector3 _lastTargetPosition;

    SphereCoords _sphereCoords; 

    private void HandleAspectUpdate(float ratio)
    {
        _aspectScaling = _baseAspectScaling * ratio / _baseAspectRatio; 
    }

    private void OnEnable()
    {
        //if (_aspectTracker)
        //{
        //    _aspectTracker.OnAspectUpdated += HandleAspectUpdate;
        //}
    }

    private void OnDisable()
    {
        //if (_aspectTracker)
        //{
        //    _aspectTracker.OnAspectUpdated -= HandleAspectUpdate;
        //}
    }

    private void Awake()
    {
        _velocity = Vector3.zero;
        // _angularVelocity = Quaternion.identity;

        _lastTargetPosition = _target.position;
        _originPosition = transform.position; 

        //if (_aspectTracker)
        //{
        //    _baseAspectRatio = _aspectTracker.AspectRatio;
        //}

        //_aspectAdapter = GetComponent<AdaptCameraSizeToAspect>();

        _sphereCoords = GetComponent<SphereCoords>();
    }


    // Update is called once per frame
    void Update()
    {
        //var flattenedPosition = transform.position;
        //flattenedPosition.z = _target.position.z;

        float scaleFactor = GetScalingFromCamera();

        var outerBounds = new Bounds(_originPosition + scaleFactor * _outerZoneOffset, scaleFactor * _outerZoneExtent);
        var innerBounds = new Bounds(_originPosition + scaleFactor * _safezoneOffset, scaleFactor * _safeZoneExtent);

        if (!_moving && !outerBounds.Contains(_target.transform.position))
        {
            _moving = true;

            // reset velocity
            if (_killAcceleration)
            {
                _velocity = Vector3.zero;
            }

            //Vector3 targetPos = _target.position;
            //targetPos.z = transform.position.z;

            //var direction = (targetPos - transform.position).normalized;
            //_velocity = direction * _maxSpeed; 

        } else if (_moving && innerBounds.Contains(_target.transform.position))
        {
            _moving = false;
        }

        if (_moving)
        {
            Vector3 targetVelocity = GetTargetVelocity();
            float damping = 1f; 

            // if camera is moving opposite the player
            // slow it down
            if (Vector3.Dot(targetVelocity, _velocity) < 0f)
            {
                damping = 1 / _catchupDampingFactor; 
            }

            Vector3 targetPos = _target.position;
            // targetPos.z = transform.position.z;

            var direction = (targetPos - _originPosition).normalized;
            var acceleration = direction * _acceleration;

            _velocity *= damping; 

            _velocity += Time.deltaTime * acceleration;


        }
        else
        {
            // if velocity is large and inner bounds are tiny
            // camera can yo-yo dizzyingly

            // also experimented with scaling acceleration / max velocity
            // by reciprocal of scale factor but results were too snappy

            if (_velocity.magnitude > innerBounds.extents.magnitude)
            {
                _velocity *= 0.3f;
            } else
            {
                _velocity *= (1f - _friction);
            }
        }

        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);

        _originPosition += _velocity * Time.deltaTime;

        _lastTargetPosition = _target.position;

        transform.position = _originPosition + _sphereCoords.GetRectFromSphere();

        Quaternion temp = transform.rotation;
        transform.LookAt(_target, Vector3.up);

        Quaternion targetRotation = transform.rotation;
        // transform.rotation = temp;

        transform.rotation = Quaternion.Slerp(temp, targetRotation, _angularLerpFactor); 

        // var dif = targetRotation * Quaternion.Inverse(transform.rotation);
        


        

    }

    Vector3 GetTargetVelocity()
    {
        return (_target.position - _lastTargetPosition) / Time.deltaTime; 
    }

    float GetScalingFromCamera()
    {
        float scaleFactor = 1f;

        // scaling based on aspect ratio
        //if (_scaleBoundsWithAspect && _aspectTracker)
        //{
        //    scaleFactor = _aspectScaling;

        //    if (_aspectAdapter)
        //    {
        //        scaleFactor /= _aspectAdapter.PortraitModeScaleFactor;
        //    }
        //}

        return scaleFactor; 
    }

    // editor visualization
    void OnDrawGizmos()
    { 
        Color lineColor = Color.yellow;

        float scaleFactor = GetScalingFromCamera();

        Vector3 pos = transform.position;

        if (Application.isPlaying)
        {
            pos = _originPosition; 
        }

        Gizmos.color = lineColor;
        Gizmos.DrawWireCube(pos + scaleFactor * _safezoneOffset, scaleFactor * _safeZoneExtent);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos + scaleFactor * _outerZoneOffset, scaleFactor * _outerZoneExtent);

    }
}
