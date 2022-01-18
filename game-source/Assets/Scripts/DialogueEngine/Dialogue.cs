using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "DialogueSystem/Dialogue", order = 1)]
public class Dialogue : ScriptableObject {
    public string label;

    [TextArea(3, 10)]
    public string[] lines;
}
