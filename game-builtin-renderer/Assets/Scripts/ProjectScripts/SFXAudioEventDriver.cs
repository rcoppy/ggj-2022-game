using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity; 

namespace GGJ2022
{
    public class SFXAudioEventDriver : MonoBehaviour
    {
        // singleton pattern
        public static SFXAudioEventDriver Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
            }

            _sfxMapFile.RefreshMap();
            _sfxDict = _sfxMapFile.Map;
        }

        [SerializeField]
        GameObject _listener;

        [SerializeField]
        SFXFMODMap _sfxMapFile;

        [SerializeField]
        Dictionary<string, FMOD.GUID> _sfxDict; 

        [SerializeField]
        RelativeCharacterController _playerController;

        [SerializeField]
        CharacterAnimationManager _playerAnimationManager;


        public static void StaticFireSFXEvent(string action)
        {
            Instance.FireSFXEvent(action);
        }

        public void FireSFXEvent(string action)
        {
            if (_sfxDict == null)
            {
                _sfxMapFile.RefreshMap();
                _sfxDict = _sfxMapFile.Map; 
            }

            FMOD.GUID guid = _sfxDict[action];

            FMODUnity.RuntimeManager.PlayOneShotAttached(guid, _listener);
        }

        private void OnEnable()
        {
            _playerController.OnJumpStarted.AddListener(() => FireSFXEvent("Jump"));
            _playerController.OnJumpEnded.AddListener(() => FireSFXEvent("EndJump"));

            _playerAnimationManager.OnActionStarted += FireSFXEvent;
        }

        private void OnDisable()
        {
            _playerController.OnJumpStarted.RemoveAllListeners();
            _playerController.OnJumpEnded.RemoveAllListeners();

            _playerAnimationManager.OnActionStarted -= FireSFXEvent;
            


        }
    }
}
