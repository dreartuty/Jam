using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;

public class RoundSetup : NetworkBehaviour
{
    public static RoundSetup Instance;

    [Header("People and places")]
    [SerializeField] public GameObject[] initialPeople;
    [SerializeField] public GameObject[] initialPlaces;
    [SerializeField] public GameObject[][] initialPlacesSpawnpoints;

    [Header("people distribution")]
    [Tooltip("people in each place sorted by place")]
    public List<GameObject>[] PeopleInPlaces;

    [Header("Case setup")]
    [Header("Clients")]
    [SerializeField] public GameObject[] clientPrefabs;
    public GameObject[] clients = new GameObject[2];

    [Header("Whitnesses Location")]
    [SerializeField] public int whitnessesLocationCount = 4;
    public List<int>[] WhitnessLocation = new List<int>[2];


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
    public List<GameObject>[] Whitnesses = new List<GameObject>[2];

    private void Awake()
    {

    }
    public override void OnNetworkSpawn()
    {
        if (!IsSessionOwner)
        {
            return;
        }
        PeopleInPlaces = new List<GameObject>[initialPlaces.Length];
        initialPlacesSpawnpoints = new GameObject[initialPlaces.Length][];
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

    public void SetupRound()
    {
        PickClients();
        SpreadNpcs();
        PickWhitnessesLocations();
        PickWhitnesses();
    }
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
        List<GameObject> initialPeopleList = new List<GameObject>(initialPeople);

        while (initialPeopleList.Count > 0)
        {
            for (int i = 0; i < initialPlaces.Length; i++)
            {
                if (initialPeopleList.Count == 0)
                    break;

                int randGen = UnityEngine.Random.Range(0, initialPeopleList.Count);
                PeopleInPlaces[i].Add(initialPeopleList[randGen]);
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
            WhitnessLocation[0].Add(locationIndex);
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
            WhitnessLocation[1].Add(locationIndex);
        }
    }
    public void PickWhitnesses()
    {
        //pick the whitnesses
        Whitnesses[0] = new List<GameObject>();
        int witCount0 = UnityEngine.Random.Range(whitnessesCount.min, whitnessesCount.max);
        Whitnesses[1] = new List<GameObject>();
        int witCount1 = UnityEngine.Random.Range(whitnessesCount.min, whitnessesCount.max);

        List<GameObject>[] PeopleInPlacesList = new List<GameObject>[initialPlaces.Length];
        for (int i = 0; i < PeopleInPlacesList.Length; i++)
        {
            PeopleInPlacesList[i] = new List<GameObject>(PeopleInPlaces[i]);
        }

        while (Whitnesses[0].Count < witCount0)
        {
            int totalAvailable = 0;
            foreach (var list in PeopleInPlacesList)
                totalAvailable += list.Count;

            if (totalAvailable == 0)
                break;

            for (int i = 0; i < WhitnessLocation[0].Count; i++)
            {

                if (Whitnesses[0].Count == witCount0)
                    break;
                if (PeopleInPlacesList[WhitnessLocation[0][i]].Count == 0)
                    continue;

                int randInt = UnityEngine.Random.Range(0, PeopleInPlacesList[WhitnessLocation[0][i]].Count);
                Whitnesses[0].Add(PeopleInPlacesList[WhitnessLocation[0][i]][randInt]);
                PeopleInPlacesList[WhitnessLocation[0][i]].RemoveAt(randInt);
            }
        }

        while (Whitnesses[1].Count < witCount1)
        {
            int totalAvailable = 0;
            foreach (var list in PeopleInPlacesList)
                totalAvailable += list.Count;

            if (totalAvailable == 0)
                break;

            for (int i = 0; i < WhitnessLocation[1].Count; i++)
            {

                if (Whitnesses[1].Count == witCount1)
                    break;
                if (PeopleInPlacesList[WhitnessLocation[1][i]].Count == 0)
                    continue;

                int randInt = UnityEngine.Random.Range(0, PeopleInPlacesList[WhitnessLocation[1][i]].Count);
                Whitnesses[1].Add(PeopleInPlacesList[WhitnessLocation[1][i]][randInt]);
                PeopleInPlacesList[WhitnessLocation[1][i]].RemoveAt(randInt);
            }
        }
    }
}