using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Profile", menuName = "NPC/NPC Profile")]
public class NPCProfile : ScriptableObject
{
    public string npcName;
    public Sprite portrait; //(poate nu folosim ca oricum i-se vede fata)
    public Color nameColor = Color.white;
    [TextArea] public string description;

    public float Temper;
    public float Reason;
    public Items favouriteItem;
    public Dialogue satisfiedDialogue;
    public Dialogue needItemDialogue;
    public Dialogue notKnowingDialogue;
}