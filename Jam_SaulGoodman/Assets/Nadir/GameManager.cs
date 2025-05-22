using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Multiplayer;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkClient client0;
    public NetworkClient client1;
    public ulong player1Id;
    public ulong player2Id;
    public NetworkVariable<Vector3> TargetPosition1 = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> TargetPosition2 = new NetworkVariable<Vector3>();

    public ISession session;

    public enum GameState { InRound, NotInRound }
    public enum RoundState {Lobby, Setup, InPrepPhase, InSearchPhase, InCourtPhase, InEndPhase }

    public RoundState currentState = RoundState.Lobby;
    public NetworkVariable<int> player1Score = new NetworkVariable<int>();
    public NetworkVariable<int> player2Score = new NetworkVariable<int>();
    public bool isInLobby = true;
    public int InitialScore = 10;

    public Transform[] player1SpawnPoints = new Transform[4];
    public Transform[] player2SpawnPoints = new Transform[4];

    [Header("SearchPhase")]

    [SerializeField] public float searchPhaseDuration = 6f; // Duration in seconds
    [SerializeField] public float preparationPhaseDuration = 3f; // Duration in seconds
    public float timeRemaining; // 10 minutes in seconds
    private bool timerIsRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
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
                TargetPosition1.Value = player1SpawnPoints[0].position;
                //TeleportPlayer(0);
                TargetPosition2.Value = player2SpawnPoints[0].position;
                currentState = RoundState.Setup;

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
                else if (currentState == RoundState.InSearchPhase)
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
        if (!IsSessionOwner)
        {
            return;
        }
        AssignPlayers();
        //TeleportPlayer(0);

        player1Score.Value = InitialScore;
        player2Score.Value = InitialScore;
        StartRound();
    }
    public void StartRound()
    {
        if (!IsSessionOwner && IsServer)
        {
            return;
        }
        RoundSetup.Instance.SetupRound();
        TargetPosition1.Value = player1SpawnPoints[2].position;
        //TeleportPlayer(0);
        TargetPosition2.Value = player2SpawnPoints[2].position;
    }
    public void StartPreparationPhase()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        //start the search phase
        TargetPosition1.Value = player1SpawnPoints[2].position;
        //TeleportPlayer(0);
        TargetPosition2.Value = player2SpawnPoints[2].position;
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
        TargetPosition1.Value = player1SpawnPoints[3].position;
        //TeleportPlayer(0);
        TargetPosition2.Value = player2SpawnPoints[3].position;
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
        TargetPosition1.Value = player1SpawnPoints[4].position;
        //TeleportPlayer(0);
        TargetPosition2.Value = player2SpawnPoints[4].position;
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

        client0 = NetworkManager.Singleton.ConnectedClientsList[0];
        client1 = NetworkManager.Singleton.ConnectedClientsList[1];

        player1Id = client0.ClientId;
        player2Id = client1.ClientId;


        Debug.Log($"AssignPlayers: P1 = {player1Id}, P2 = {player2Id}");
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
        currentState = RoundState.Lobby;
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
