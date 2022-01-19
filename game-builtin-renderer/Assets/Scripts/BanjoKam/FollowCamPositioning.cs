using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamPositioning : MonoBehaviour {

    public Transform targetTransform;
    public float minLookAheadDistance = 0.5f;
    public float lookAheadDistance = 4f;
    public float lerpFactor = 0.05f;

    [SerializeField]
    bool _shouldAvoidGeometry = true; 

    [Header("SphereCoords Vars")]
    public float yawSpeed = 15f;
    public float pitchSpeed = 15f;
    public float maxPitch = 75f;
    public float minPitch = 10f;
    public float maxRadius = 9f;
    public float minRadius = 1f;
    public float idealRadius = 9f;

    private SphereCoords coords;
    private Camera cam;
    private Vector2 moveDirection;
    private Vector3 myPosition; // transform to which spherical coords are applied--this camera's origin
    private Vector3 lookTargetPosition;

    // public bool isFrozen = false; 

    void Awake()
    {
        myPosition = transform.position;
        lookTargetPosition = targetTransform.position;
        moveDirection = new Vector2();
    }

    // Use this for initialization
    void Start() {
        coords = GetComponent<SphereCoords>();
        cam = GetComponent<Camera>();

    }

    // Update is called once per frame
    void LateUpdate() {

        bool isFrozen = false; // PlayerState.instance.GetIsInputFrozen(); 

        if (!isFrozen)
        {
            HandleInput();
            Move();
            Orient();

            if (_shouldAvoidGeometry)
            {
                AvoidGeometry();
            }
            
        }
    }

    void HandleInput()
    {
#if UNITY_N3DS && !UNITY_EDITOR
        Vector2 circlePad = UnityEngine.N3DS.GamePad.CirclePad;
        moveDirection = new Vector2
        {
            x = circlePad.x, 
            y = circlePad.y
        };
#else
        moveDirection = new Vector2
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };
#endif
    }

    void Move()
    {
        coords.yaw -= moveDirection.x * yawSpeed * Time.deltaTime;
        coords.pitch -= moveDirection.y * pitchSpeed * Time.deltaTime;

        coords.pitch = Mathf.Clamp(coords.pitch, minPitch, maxPitch);

        // myPosition is Origin
        myPosition = Vector3.Lerp(myPosition, targetTransform.position, lerpFactor);

        // apply sphere coords relative to origin
        transform.position = myPosition + coords.GetRectFromSphere();

    }

    void Orient()
    {
        // lookat
        Vector3 lookOffset;
        float maxDist = lookAheadDistance;


        // if moving towards the camera (y-axis negative), shrink lookahead distance to keep player in view
        if (moveDirection.y < -0.15f)
        {
            maxDist = minLookAheadDistance * 1.3f;
        }
        else if (moveDirection.y > 0.5f)
        {
            maxDist = lookAheadDistance * 1.6f;
        }

        lookOffset = targetTransform.forward * Mathf.Lerp(minLookAheadDistance, maxDist, moveDirection.normalized.magnitude);


        Vector3 camLookTarget = targetTransform.position + lookOffset;

        // only adjust if min threshold passed? 

        lookTargetPosition = Vector3.Lerp(lookTargetPosition, camLookTarget, lerpFactor);

        Debug.DrawLine(targetTransform.position, camLookTarget);

        transform.LookAt(lookTargetPosition);
    }

    void AvoidGeometry()
    {
        // this works, technically--but the implementation doesn't solve the right problem
        // one ray--player to cam
        // 4 rays-- player to viewport corners in worldspace
        // radius is sphere coords radius

        if (CheckIsPlayerOccluded())
        {
            // shrink radius
            coords.radius = Mathf.Lerp(coords.radius, minRadius, lerpFactor);
            print("Occluded");
        }
        else
        {
            // grow radius to ideal
            float old = coords.radius; 

            coords.radius = Mathf.Lerp(coords.radius, idealRadius, lerpFactor);

            if (CheckIsPlayerOccluded())
            {
                coords.radius = old; // discard change; it just occluded the view again
            }
        }
    }

    bool CheckIsPlayerOccluded() { 
        Vector3[] corners = new Vector3[4];
        corners[0] = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
        corners[1] = cam.ViewportToWorldPoint(new Vector3(0f, 1f, cam.nearClipPlane));
        corners[2] = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));
        corners[3] = cam.ViewportToWorldPoint(new Vector3(1f, 0f, cam.nearClipPlane));

        bool IsPlayerOccluded = false; // Physics.Raycast(targetTransform.position, (transform.position - targetTransform.position).normalized, coords.radius);

        for (int i = 0; i < corners.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(targetTransform.position, (targetTransform.position - corners[i]).normalized, out hit, coords.radius)
                && hit.collider.gameObject != targetTransform.gameObject)
            {
                IsPlayerOccluded = true;
                Debug.Log(hit.collider.gameObject.name);
                break;
            }
        }

        return IsPlayerOccluded;
    }

}
