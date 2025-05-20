using UnityEngine;
using Unity.Netcode;



public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
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
    }
}
