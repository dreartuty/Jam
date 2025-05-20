using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Multiplayer;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public ulong player1Id;
    public ulong player2Id;
    public ISession session;

    public enum GameState { InRound, NotInRound }
    public enum RoundState { Setup, InPrepPhase, InSearchPhase, InCourtPhase, InEndPhase }

    public RoundState currentState = RoundState.Setup;
    public NetworkVariable<int> player1Score = new NetworkVariable<int>();
    public NetworkVariable<int> player2Score = new NetworkVariable<int>();
    public bool isInLobby = true;
    public int InitialScore = 10;

    [Header("SearchPhase")]

    [SerializeField] public float searchPhaseDuration = 6f; // Duration in seconds
    [SerializeField] public float preparationPhaseDuration = 3f; // Duration in seconds
    public float timeRemaining; // 10 minutes in seconds
    private bool timerIsRunning = false;

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
        if (!isInLobby && NetworkManager.Singleton.ConnectedClients.Count < 2)
        {
            ReturnToLobby();
        }
        if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    //DisplayTime(timeRemaining);
                }
                else
                {
                    Debug.Log("Time's up!");
                    timeRemaining = 0;
                    timerIsRunning = false;
                    if (currentState == RoundState.InPrepPhase)
                    {
                        currentState = RoundState.InSearchPhase;
                        StartSearchPhase();
                        return;
                    }
                    if (currentState == RoundState.InSearchPhase)
                    {
                        currentState = RoundState.InCourtPhase;
                        StartCourtPhase();
                        return;
                    }
                }
            }
    }
    public void SetupMatch()
    {
        if (!IsSessionOwner && IsServer)
        {
            return;
        }
        AssignPlayers();
        player1Score.Value = InitialScore;
        player2Score.Value = InitialScore;
        StartRound();
    }
    public void StartRound()
    {
        RoundSetup.Instance.SetupRound();
    }
    public void StartPreparationPhase()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        //start the search phase
        timeRemaining = preparationPhaseDuration;
        timerIsRunning = true;
        currentState = RoundState.InPrepPhase;
    }

    public void StartSearchPhase()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        //start the search phase
        timeRemaining = searchPhaseDuration;
        timerIsRunning = true;
        currentState = RoundState.InSearchPhase;

    }
    public void StartCourtPhase()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        currentState = RoundState.InCourtPhase;
        timerIsRunning = false;

        //start the court phase
    }

    public void AssignPlayers()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        // Assign players based on the connected clients
        var clients = NetworkManager.Singleton.ConnectedClientsList;
        if (clients.Count == 2)
        {
            player1Id = clients[0].ClientId;
            player2Id = clients[1].ClientId;
            Debug.Log($"Player 1 is {player1Id}, Player 2 is {player2Id}");
        }
    }
    
    public ulong GetPlayer1Id()
    {
        return player1Id;
    }

    public ulong GetPlayer2Id()
    {
        return player2Id;
    }   
    public void ReturnToLobby()
    {
        if (!IsSessionOwner)
            return;

        isInLobby = true;
        currentState = RoundState.Setup;
        player1Score.Value = 0;
        player2Score.Value = 0;
        timeRemaining = 0;
        timerIsRunning = false;

        // Shutdown network and destroy the NetworkManager manually
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);

        // Now load the lobby scene (index 0)
        SceneManager.LoadScene(0);
    }
}
