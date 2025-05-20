using UnityEngine;
using Unity.Netcode;



public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public enum GameState {InRound, NotInRound}
    public NetworkVariable<int> player1Score = new NetworkVariable<int>();
    public NetworkVariable<int> player2Score = new NetworkVariable<int>();
    public bool isInLobby = true;

    public int InitialScore = 10; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        Instance = this;
    }
    public void OnSessionOwnerPromoted(ulong sessionOwnerId)
    {
        if (!IsSessionOwner)
        {
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSpawned || !IsSessionOwner)
        {
            return;
        }
        if (isInLobby)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                isInLobby = false;
                SetupMatch();
            }
        }
    }
    public void SetupMatch()
    {
        if (!IsSessionOwner && IsServer)
        {
            return;
        }
        player1Score.Value = InitialScore;
        player2Score.Value = InitialScore;
        StartRound();
    }
    public void StartRound()
    {
        RoundSetup.Instance.SetupRound();
    }
}
