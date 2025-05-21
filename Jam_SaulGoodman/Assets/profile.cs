using UnityEngine;
using Unity.Netcode;

public class profile : NetworkBehaviour
{
    public NPCProfile Profile;
    public enum Whitness { player0, player1, none }
    public Whitness CrimeKnowledge = Whitness.none;
    public bool wasGivenItem = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
