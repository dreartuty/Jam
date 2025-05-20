using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

public class CourtPlay : NetworkBehaviour
{
    public static CourtPlay Instance;

    public enum PlayerTurn { Player1, Player2 }
    public NetworkVariable<int> player1Bid = new NetworkVariable<int>();
    public NetworkVariable<int> player2Bid = new NetworkVariable<int>();
    public NetworkVariable<PlayerTurn> currentTurn = new NetworkVariable<PlayerTurn>();

    public int player1Money = 10;
    public int player2Money = 10;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsSessionOwner) return;
        {
            ResetBidding();
        }
    }

    void ResetBidding()
    {
        player1Bid.Value = 0;
        player2Bid.Value = 0;
        currentTurn.Value = PlayerTurn.Player1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitBidServerRpc(ulong clientId, int amount)
    {
        if (!IsSessionOwner) return;

        if (currentTurn.Value == PlayerTurn.Player1 && clientId == GetPlayer1Id())
        {
            if (amount > player1Money) return;
            player1Bid.Value += amount;
            player1Money -= amount;
            EvaluateTurn();
        }
        else if (currentTurn.Value == PlayerTurn.Player2 && clientId == GetPlayer2Id())
        {
            if (amount > player2Money) return;
            player2Bid.Value += amount;
            player2Money -= amount;
            EvaluateTurn();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void foldBiddingServerRpc(ulong clientId)
    {
        if (!IsSessionOwner) return;
        if (clientId == GetPlayer1Id())
        {
            player1Bid.Value = 0;
        }
        else if (clientId == GetPlayer2Id())
        {
            player2Bid.Value = 0;
        }
        Compare();
    }

    void EvaluateTurn()
    {
        if (player1Bid.Value > player2Bid.Value)
            currentTurn.Value = PlayerTurn.Player2;
        else if (player2Bid.Value > player1Bid.Value)
            currentTurn.Value = PlayerTurn.Player1;
        else
            currentTurn.Value = currentTurn.Value == PlayerTurn.Player1 ? PlayerTurn.Player2 : PlayerTurn.Player1;

        CheckEnd();
    }

    void CheckEnd()
    {
        if (!IsSessionOwner) return;
        if (player1Money == 0 && player1Bid.Value < player2Bid.Value)
        {
            Debug.Log("Player 1 can't match — Player 2 wins the bid!");
            GameManager.Instance.player2Score.Value += player2Bid.Value;
            GameManager.Instance.player1Score.Value -= player1Bid.Value;

        }
        else if (player2Money == 0 && player2Bid.Value < player1Bid.Value)
        {
            Debug.Log("Player 2 can't match — Player 1 wins the bid!");
            GameManager.Instance.player1Score.Value += player1Bid.Value;
            GameManager.Instance.player2Score.Value -= player2Bid.Value;


        }
    }
    void Compare()
    {
        if (!IsSessionOwner) return;
        if (player1Bid.Value < player2Bid.Value)
        {
            Debug.Log("Player 1 pleads guilty — Player 2 wins the bid!");
            GameManager.Instance.player2Score.Value += player2Bid.Value;
            GameManager.Instance.player1Score.Value -= player1Bid.Value;

        }
        else if (player2Bid.Value < player1Bid.Value)
        {
            Debug.Log("Player 2 pleads guilty — Player 1 wins the bid!");
            GameManager.Instance.player1Score.Value += player1Bid.Value;
            GameManager.Instance.player2Score.Value -= player2Bid.Value;
        }
    }

    ulong GetPlayer1Id() => NetworkManager.Singleton.ConnectedClientsIds.Count > 0
        ? NetworkManager.Singleton.ConnectedClientsIds[0]
        : 0;

    ulong GetPlayer2Id() => NetworkManager.Singleton.ConnectedClientsIds.Count > 1
        ? NetworkManager.Singleton.ConnectedClientsIds[1]
        : 0;
}