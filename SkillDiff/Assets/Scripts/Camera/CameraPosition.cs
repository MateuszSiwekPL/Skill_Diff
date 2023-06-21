using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class CameraPosition : NetworkBehaviour
{
    [SerializeField] Transform player;

    public override void OnNetworkSpawn() {
        player = GameObject.Find("Player(clone)").transform;
    }
    
    void Update()
    {
        //transform.position = player.position;
    }
}
