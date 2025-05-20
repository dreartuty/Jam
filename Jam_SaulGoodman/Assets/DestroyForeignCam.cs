using UnityEngine;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;

public class DestroyForeignCam : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (!HasAuthority){Destroy(gameObject);}
    }
}
