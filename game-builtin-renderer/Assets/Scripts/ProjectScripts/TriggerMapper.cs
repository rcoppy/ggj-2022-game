using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GGJ2022
{
    // useful for scripting zone-based events in-editor
    public class TriggerMapper : MonoBehaviour
    {
        [SerializeField]
        UnityEvent OnTriggerActivated;

        [SerializeField]
        UnityEvent OnTriggerDeactivated;

        [Header("Which transforms activate trigger?")]
        [SerializeField]
        List<Transform> _triggeringTransforms;

        // prevent double trigger (if player object has multiple colliders)
        int i = 0;

        private void Awake()
        {
            i = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (i == 0 && _triggeringTransforms.Contains(other.transform))
            {
                OnTriggerActivated?.Invoke();
            }
            i++;
        }

        private void OnTriggerExit(Collider other)
        {
            if (_triggeringTransforms.Contains(other.transform))
            {
                OnTriggerDeactivated?.Invoke();
                i = 0; 
            }
        }
    }
}