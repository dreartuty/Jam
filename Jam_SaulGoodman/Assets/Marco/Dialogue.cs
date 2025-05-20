using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue")]

// a full dialogue sequence held by an NPC
public class Dialogue : ScriptableObject
{
    public NPCProfile profile;
    public DialogueLine[] lines;
}

// a single line of dialogue spoken by an NPC
[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea] public string line;
}