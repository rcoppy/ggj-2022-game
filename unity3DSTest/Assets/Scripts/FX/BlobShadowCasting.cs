using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobShadowCasting : MonoBehaviour {

    // updates transform of blob shadow (attach it to the shadow plane)
    // if no parent transform found, destroys self

    // public float maxShadowRadius = 2.5f;
    [Tooltip("max distance to cast ray downward before shadow is disabled")]
    public float raycastDepth = 5f;
    [Tooltip("Shadow will grow up to x times its original size")]
    public float maxShadowGrowth = 3f;
    [Tooltip("collision layer to check against")]
    public string layerName = "LevelGeometry";
    private MeshRenderer meshRenderer;
    private Vector3 originalScale;

	// Use this for initialization
	void Start () {
        if (transform.parent == null)
        {
            Destroy(gameObject); // shadow must have a casting parent
        }

        meshRenderer = GetComponent<MeshRenderer>();
        originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {


        int layerMask = 1 << LayerMask.NameToLayer(layerName); 

        RaycastHit hit;

        if (Physics.Raycast(transform.parent.transform.position, Vector3.down, out hit, raycastDepth, layerMask))
        {
            meshRenderer.enabled = true;
            transform.position = hit.point + new Vector3(0f, 0.015f, 0f);
            transform.localScale = originalScale * (1 + hit.distance / raycastDepth * (maxShadowGrowth-1)); // shadow gets bigger farther away from ground
        }
        else
        {
            meshRenderer.enabled = false; 
        }

        Debug.DrawRay(transform.parent.transform.position, Vector3.down * hit.distance, Color.yellow);
    }
}
