using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Profile", menuName = "NPC/NPC Profile")]
public class NPCProfile : ScriptableObject
{
    public enum Whitness{player0, player1, none}
    public Whitness CrimeKnowledge;
    public string npcName;
    public RenderTexture portrait; //(poate nu folosim ca oricum i-se vede fata)
    public Color nameColor;
    [TextArea] public string description;

    public float Temper;
    public float Reason;
    public Items favouriteItem;
}