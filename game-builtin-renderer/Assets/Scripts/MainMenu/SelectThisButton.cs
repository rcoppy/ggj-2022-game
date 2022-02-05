using System.Collections;
using System.Collections.Generic;
using GGJ2022.Audio;
using UnityEngine;
using UnityEngine.UI;

public class SelectThisButton : MonoBehaviour
{
  
    public void Select()
    {
        gameObject.GetComponent<Button>().Select();
        SFXAudioEventDriver.Instance.FireSFXEvent("UiInteraction");
    }
}
