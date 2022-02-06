using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2022.FX
{
    public class ChangeHamsMaterial : MonoBehaviour
    {
        [SerializeField] Material _defaultTexture;
        [SerializeField] Material _deathTexture;

        private Material[] materials;
        private SkinnedMeshRenderer[] renderers;

        // Start is called before the first frame update
        void Start()
        {
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            GGJ2022.EnemyAI.PlayerState.OnDied += Death;
        }

        void Death()
        {
            ChangeMaterial(_deathTexture);
        }

        void ChangeMaterial(Material newMat)
        {
            foreach (var r in renderers)
            {
                var m = r.materials;
                m[0] = newMat;

                r.materials = m;
            }

            Debug.Log($"{renderers.Length} materials changed");
        }
    }
}