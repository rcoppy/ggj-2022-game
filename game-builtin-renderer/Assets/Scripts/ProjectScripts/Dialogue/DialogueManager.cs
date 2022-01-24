using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;
using GGJ2022.Dialogue.Schema;
using TMPro; 

// see these tutorials:
// https://www.youtube.com/watch?v=mXjRR1nnC5M
// https://www.youtube.com/watch?v=_nRzoTzeyxU

namespace GGJ2022.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {

        // singleton pattern
        public static DialogueManager instance { get; set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        Queue<Speech> exchanges;
        Queue<Line> dialogueLines; 
        Speech _activeSpeech; 


        public GameObject DialogueCanvas;

        [SerializeField]
        private TextMeshProUGUI _textPanel;

        [SerializeField]
        private TextMeshProUGUI _namePanel;

        [SerializeField]
        private RawImage _portraitPanel;

        [SerializeField]
        private CharacterBank _characterBank;

        private bool isDialogueActive = false;
        // public InputAction continueButton;

        Cutscene _activeCutscene; 

        public bool GetIsDialogueActive()
        {
            return isDialogueActive;
        }

        private void Start()
        {
            // textObject = DialogueCanvas.GetComponentInChildren<TextMeshProUGUI>();
            DialogueCanvas.SetActive(false);
            _characterBank.RefreshMap();
        }

        public void StartNewDialogue(Cutscene cutscene)
        {
            // queue: first-in, first-out
            // first line of dialogue to be added will be first line to be returned
            isDialogueActive = true;

            exchanges = new Queue<Speech>(cutscene.Exchanges);
            dialogueLines = new Queue<Line>();

            _activeCutscene = cutscene; 

            cutscene.Pre?.Invoke();

            DialogueCanvas.SetActive(true);

            UpdateDialogue(); // step through one line

            // PlayerState.instance.FreezeInput();
        }

        public void UpdateDialogue()
        {
            if (dialogueLines.Count > 0)
            {
                Line line = dialogueLines.Dequeue(); // will return and remove oldest (first added) element

                _textPanel.text = line.Text;

                Debug.Log(line.Text);
            }
            else if (exchanges.Count > 0)
            {
                _activeSpeech = exchanges.Dequeue();

                dialogueLines = new Queue<Line>(_activeSpeech.Lines);

                // UI update
                _namePanel.text = _activeSpeech.Speaker;
                _portraitPanel.texture = _characterBank.CharacterMap[_activeSpeech.Speaker].Photo;

                UpdateDialogue();
            } else { 

                // close the dialogue
                DialogueCanvas.SetActive(false);
                isDialogueActive = false;

                // assume dialogue was triggered by an interaction; end that interaction
                //if (Interactable.isInteractionInProgress)
                //{
                Interactable.EndInteraction();
                Debug.Log("isinteract false");
                //} // end that interaction (hackey; not ideal; fix this)

                // PlayerState.instance.UnfreezeInput();
            }
        }

        // when fire1 button is pressed, continue the active dialogue 

        //bool waitFlag = false; // need to wait for a frame, otherwise first speech line gets skipped 

        //void Update()
        //{
        //    if (DialogueManager.instance.GetIsDialogueActive())
        //    {
        //        if (waitFlag)
        //        {
        //            if (Input.GetButtonDown(continueButton))
        //            {
        //                DialogueManager.instance.UpdateDialogue();
        //            }
        //        }
        //        else
        //        {
        //            waitFlag = true;
        //        }
        //    }
        //    else
        //    {
        //        waitFlag = false; // reset flag
        //    }
        //}

        public void ProgressDialogue(InputAction.CallbackContext context)
        {
            if (context.performed && DialogueManager.instance.GetIsDialogueActive())
            {
                DialogueManager.instance.UpdateDialogue();
            }
        }
    }
}