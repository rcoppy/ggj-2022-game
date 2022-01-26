using UnityEngine;
using System.Collections.Generic;
using System;

namespace GGJ2022
{
    [CreateAssetMenu(fileName = "SFXMap", menuName = "GGJ2022/SFX event map", order = 1)]
    public class SFXFMODMap : ScriptableObject
    {
        [Serializable]
        public struct KeyValue
        {
            [SerializeField]
            public string Action;

            [SerializeField]
            public FMODUnity.EventReference FMODEvent;
        }

        [SerializeField]
        List<KeyValue> _keyValues;

        Dictionary<string, FMODUnity.EventReference> _sfxMap;

        public Dictionary<string, FMODUnity.EventReference> Map {
            get { return _sfxMap; }
        }

        public void RefreshMap()
        {
            _sfxMap = new Dictionary<string, FMODUnity.EventReference>();

            foreach (var kvp in _keyValues) {
                _sfxMap[kvp.Action] = kvp.FMODEvent;
            }

        }
    }
}
