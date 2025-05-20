using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class RoundSetup : NetworkBehaviour
{
    public static RoundSetup Instance;

    [Header("People and places")]
    [SerializeField] public GameObject[] initialPeople;
    [SerializeField] public GameObject[] initialPlaces;

    [System.Serializable]
    public class initialSpawnpoints
    {
        public GameObject[] spawnpoints;
    }
    public initialSpawnpoints[] initialPlacesSpawnpoints;

    [Header("people distribution")]
    [Tooltip("people in each place sorted by place")]
    public List<GameObject>[] PeopleInPlaces;

    [Header("Case setup")]
    [Header("Clients")]
    [SerializeField] public GameObject[] clientPrefabs;
    public GameObject[] clients = new GameObject[2];

    [Header("Whitnesses Location")]
    [SerializeField] public int whitnessesLocationCount = 4;
    public List<int> WhitnessLocation0 = new List<int>();
    public List<int> WhitnessLocation1 = new List<int>();

    [Header("Whitnesses")]
    [SerializeField] public Range whitnessesCount = new Range(6, 10);
    [System.Serializable]
    public class Range
    {
        public int min;
        public int max;

        public Range(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
    public List<GameObject> Whitnesses0 = new List<GameObject>();
    public List<GameObject> Whitnesses1 = new List<GameObject>();


    private void Awake()
    {

    }
    public override void OnNetworkSpawn()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        Instance = this;
        PeopleInPlaces = new List<GameObject>[initialPlaces.Length];
    }
    public void OnSessionOwnerPromoted(ulong sessionOwnerId)
    {
        if (!IsSessionOwner)
        {
            return;
        }
        Instance = this;
    }

    public void SetupRound()
    {
        PickClients();
        SpreadNpcs();
        PickWhitnessesLocations();
        PickWhitnesses();
        SetupOver();
    }
    #region setup
    public void PickClients()
    {
        //pick the clients
        List<GameObject> clientPrefabsList = new List<GameObject>(clientPrefabs);

        int choice1 = UnityEngine.Random.Range(0, clientPrefabsList.Count);
        clients[0] = clientPrefabsList[choice1];
        clientPrefabsList.RemoveAt(choice1);
        int choice2 = UnityEngine.Random.Range(0, clientPrefabsList.Count);
        clients[1] = clientPrefabsList[choice2];
        clientPrefabsList.RemoveAt(choice2);
        clientPrefabsList = null;
    }
    public void SpreadNpcs()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        List<GameObject> initialPeopleList = new List<GameObject>(initialPeople);

        while (initialPeopleList.Count > 0)
        {
            for (int i = 0; i < initialPlaces.Length; i++)
            {
                if (initialPeopleList.Count == 0)
                    break;

                int randGen = UnityEngine.Random.Range(0, initialPeopleList.Count);
                if (PeopleInPlaces[i] == null)
                    PeopleInPlaces[i] = new List<GameObject>();
                PeopleInPlaces[i].Add(initialPeopleList[randGen]);
                NetworkObject npcInstance = Instantiate(initialPeopleList[randGen], initialPlacesSpawnpoints[i].spawnpoints[PeopleInPlaces[i].Count - 1].transform.position, Quaternion.identity).GetComponent<NetworkObject>();
                npcInstance.Spawn();
                PeopleInPlaces[i][PeopleInPlaces[i].Count - 1] = npcInstance.gameObject;
                initialPeopleList.RemoveAt(randGen);
            }
        }
    }
    public void PickWhitnessesLocations()
    {
        List<int> availableLocationsIndexes0 = new List<int>();
        for (int i = 0; i < initialPlaces.Length; i++)
        {
            availableLocationsIndexes0.Add(i);
        }

        for (int i = 0; i < whitnessesLocationCount; i++)
        {
            int randGen = UnityEngine.Random.Range(0, availableLocationsIndexes0.Count);
            int locationIndex = availableLocationsIndexes0[randGen];
            availableLocationsIndexes0.RemoveAt(randGen);
            WhitnessLocation0.Add(locationIndex);
        }

        List<int> availableLocationsIndexes1 = new List<int>();
        for (int i = 0; i < initialPlaces.Length; i++)
        {
            availableLocationsIndexes1.Add(i);
        }

        for (int i = 0; i < whitnessesLocationCount; i++)
        {
            int randGen = UnityEngine.Random.Range(0, availableLocationsIndexes1.Count);
            int locationIndex = availableLocationsIndexes1[randGen];
            availableLocationsIndexes1.RemoveAt(randGen);
            WhitnessLocation1.Add(locationIndex);
        }
    }
    public void PickWhitnesses()
    {
        //pick the whitnesses
        Whitnesses0 = new List<GameObject>();
        System.Random rng0 = new System.Random(Guid.NewGuid().GetHashCode());
        int witCount0 = rng0.Next(whitnessesCount.min, whitnessesCount.max + 1);
        Debug.Log(witCount0);
        Whitnesses1 = new List<GameObject>();
        System.Random rng1 = new System.Random(Guid.NewGuid().GetHashCode());
        int witCount1 = rng1.Next(whitnessesCount.min, whitnessesCount.max + 1);
        Debug.Log(witCount1);

        List<GameObject>[] PeopleInPlacesList = new List<GameObject>[initialPlaces.Length];
        for (int i = 0; i < PeopleInPlacesList.Length; i++)
        {
            PeopleInPlacesList[i] = new List<GameObject>(PeopleInPlaces[i]);
        }

        while (Whitnesses0.Count < witCount0)
        {
            int totalAvailable = 0;
            foreach (var list in PeopleInPlacesList)
                totalAvailable += list.Count;

            if (totalAvailable == 0)
                break;

            for (int i = 0; i < WhitnessLocation0.Count; i++)
            {

                if (Whitnesses0.Count == witCount0)
                    break;
                if (PeopleInPlacesList[WhitnessLocation0[i]].Count == 0)
                    continue;

                int randInt = UnityEngine.Random.Range(0, PeopleInPlacesList[WhitnessLocation0[i]].Count);
                Whitnesses0.Add(PeopleInPlacesList[WhitnessLocation0[i]][randInt]);
                PeopleInPlacesList[WhitnessLocation0[i]][randInt].GetComponent<profile>().CrimeKnowledge = profile.Whitness.player0;
                PeopleInPlacesList[WhitnessLocation0[i]].RemoveAt(randInt);
            }
        }

        while (Whitnesses1.Count < witCount1)
        {
            int totalAvailable = 0;
            foreach (var list in PeopleInPlacesList)
                totalAvailable += list.Count;

            if (totalAvailable == 0)
                break;

            for (int i = 0; i < WhitnessLocation1.Count; i++)
            {

                if (Whitnesses1.Count == witCount1)
                    break;
                if (PeopleInPlacesList[WhitnessLocation1[i]].Count == 0)
                    continue;

                int randInt = UnityEngine.Random.Range(0, PeopleInPlacesList[WhitnessLocation1[i]].Count);
                Whitnesses1.Add(PeopleInPlacesList[WhitnessLocation1[i]][randInt]);
                PeopleInPlacesList[WhitnessLocation1[i]][randInt].GetComponent<profile>().CrimeKnowledge = profile.Whitness.player1;
                PeopleInPlacesList[WhitnessLocation1[i]].RemoveAt(randInt);
            }
        }
    }
    #endregion

    public void SetupOver()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        
        GameManager.Instance.StartPreparationPhase();
    }

    
    
    
}
