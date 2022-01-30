using UnityEngine;
using System.Collections;
using GGJ2022.Dialogue.Schema;
using GGJ2022.Dialogue;
using FMODUnity;
using FMOD.Studio;

namespace GGJ2022.Audio
{
    public class FMODDialogueAudio : MonoBehaviour
    {
        // singleton pattern
        public static FMODDialogueAudio Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        EventInstance _speechEvent; 

        [SerializeField]
        string _UIEventPath = "event:/SFX/UI_Sound"; 

        GameObject _listener; 

        // Use this for initialization
        void Start()
        {
            _listener = TransformCamManager.instance.TargetCamera;

            Dialogue.DialogueManager.instance.OnDialogueStarted += HandleDialogueStarted;
            Dialogue.DialogueManager.instance.OnDialogueEnded += HandleDialogueEnded;
            Dialogue.DialogueManager.instance.OnDialogueSpeechProgressed += HandleSpeechProgressed;
            Dialogue.DialogueManager.instance.OnDialogueLineProgressed += HandleLineProgressed;
            Dialogue.DialogueManager.instance.TextPanel.OnTextDoneIterating += HandleTextCrawlEnded;
        }

        // private void OnEnable()
        // {
        //     try
        //     {
        //         Dialogue.DialogueManager.instance.OnDialogueStarted += HandleDialogueStarted;
        //         Dialogue.DialogueManager.instance.OnDialogueEnded += HandleDialogueEnded;
        //         Dialogue.DialogueManager.instance.OnDialogueSpeechProgressed += HandleSpeechProgressed;
        //         Dialogue.DialogueManager.instance.OnDialogueLineProgressed += HandleLineProgressed;
        //         Dialogue.DialogueManager.instance.TextPanel.OnTextDoneIterating += HandleTextCrawlEnded;
        //     } catch
        //     {
        //         Debug.LogError("Couldn't find dialogue manager instance");
        //     }
        // }

        private void OnDisable()
        {
            try
            {
                Dialogue.DialogueManager.instance.OnDialogueStarted -= HandleDialogueStarted;
                Dialogue.DialogueManager.instance.OnDialogueEnded -= HandleDialogueEnded;
                Dialogue.DialogueManager.instance.OnDialogueSpeechProgressed -= HandleSpeechProgressed;
                Dialogue.DialogueManager.instance.OnDialogueLineProgressed -= HandleLineProgressed;
                Dialogue.DialogueManager.instance.TextPanel.OnTextDoneIterating -= HandleTextCrawlEnded;
            }
            catch
            {
                Debug.LogError("Couldn't find dialogue manager instance");
            }
        }

        void HandleLineProgressed(Line line)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached(_UIEventPath, _listener);

            try
            {
                _speechEvent.setParameterByName("Dialogue End", 0f);
            }
            catch
            {
                // _speechEvent was unset
            }
        }

        void HandleSpeechProgressed(Speech speech)
        {
            try
            {
                _speechEvent.setParameterByName("Dialogue End", 1f);
            } catch
            {
                // _speechEvent was unset
            }

            string path = Dialogue.DialogueManager.instance.Characters.CharacterMap[speech.Speaker].SoundPath;
            _speechEvent = FMODUnity.RuntimeManager.CreateInstance(path);
            _speechEvent.setParameterByName("Dialogue End", 0f);

            _speechEvent.start(); 
        }

        void HandleTextCrawlEnded()
        {
            try
            {
                _speechEvent.setParameterByName("Dialogue End", 1f);
                _speechEvent.release();
            }
            catch
            {
                // _speechEvent was unset
            }
        }

        void HandleDialogueStarted(Cutscene cutscene)
        {
            // nothing for now
        }

        void HandleDialogueEnded()
        {
            try
            {
                _speechEvent.setParameterByName("Dialogue End", 1f);
                _speechEvent.release();
            }
            catch
            {
                // _speechEvent was unset
            }
        }
    }
}
