using Unity.Netcode;
using UnityEngine;

public class NetworkObjectSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject go;

    public override void OnNetworkSpawn()
    {
        if (IsSessionOwner) // works for Host too
        {
            go.GetComponent<NetworkObject>().Spawn();
            Debug.Log("Spawned network object from server.");
        }
    }
}