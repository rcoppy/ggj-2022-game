using UnityEngine;
using System.Collections;
using TMPro;

namespace GGJ2022.Dialogue
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextIterator : MonoBehaviour
    {
        TextMeshProUGUI _text;

        [SerializeField]
        float _secondsToWait = 0.02f; // 20 milliseconds

        Coroutine _coroutine;

        [SerializeField]
        bool _shouldIterate = true;

        public delegate void TextDoneIterating();
        public TextDoneIterating OnTextDoneIterating;

        // Use this for initialization
        void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void TriggerNewText(string text)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            if (_shouldIterate)
            {
                _coroutine = StartCoroutine(IterateLetters(text));
            } else
            {
                _text.text = text; 
            }
        }

        IEnumerator IterateLetters(string text)
        {
            _text.text = "";

            foreach (char c in text)
            {
                _text.text += c; 
                yield return new WaitForSeconds(_secondsToWait);
            }

            OnTextDoneIterating?.Invoke();
            yield break;
        }
    }
}
