using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHamsMaterial : MonoBehaviour
{
    [SerializeField] GameObject[] _bodyParts;
    [SerializeField] Material _defaultTexture;
    [SerializeField] Material _deathTexture;

    Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        materials = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        materials[0] = _defaultTexture;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0] = materials[0];
        GGJ2022.EnemyAI.PlayerState.OnDied += Death;
    }

    void Death()
    {
        ChangeMaterial(_deathTexture);
    }

    void ChangeMaterial(Material newMat)
    {
        materials[0] = newMat;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0] = materials[0];
    }

}
//m_Materials.Array.data[0]