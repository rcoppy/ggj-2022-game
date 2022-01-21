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

        private void OnTriggerEnter(Collider other)
        {
            if (_triggeringTransforms.Contains(other.transform))
            {
                OnTriggerActivated?.Invoke();
            }
        }

        private void OnTriggerLeave(Collider other)
        {
            if (_triggeringTransforms.Contains(other.transform))
            {
                OnTriggerDeactivated?.Invoke();
            }
        }
    }
}