using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores the lines of dialogue; passed by a dialogue trigger to the manager
[System.Serializable]
public class DialogueInstance {

    public string label;

    [TextArea(3, 10)]
    public string[] lines; 
	
}
