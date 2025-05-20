using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Profile", menuName = "NPC/NPC Profile")]
public class NPCProfile : ScriptableObject
{
    public string npcName;
    public Sprite portrait;
    [TextArea] public string description;
    public Color nameColor;
    public int friendshipLevel;
}