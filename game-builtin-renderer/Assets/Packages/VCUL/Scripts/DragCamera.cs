using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCamera : MonoBehaviour
{
    [Header("Camera settings")]
    [Tooltip("Movement speed based on mouse drag")]
    public float dragSpeed = 2;
    public bool invertMovement = false;
    private Vector3 dragOrigin;
    private Vector3 movement;
    private Vector3 worldPosition;

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        worldPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        if (invertMovement)
        {
            movement = new Vector3(-worldPosition.x * dragSpeed, 0, -worldPosition.y * dragSpeed);
        }
        else
        {
            movement = new Vector3(worldPosition.x * dragSpeed, 0, worldPosition.y * dragSpeed);
        }

        transform.Translate(movement, Space.World);
    }
}
