using UnityEngine;
using Unity.Netcode;

public class profile : NetworkBehaviour
{
    public NPCProfile Profile;
    public enum Whitness { player0, player1, none }
    public Whitness CrimeKnowledge = Whitness.none;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
