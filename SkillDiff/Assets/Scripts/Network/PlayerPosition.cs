using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPosition : NetworkBehaviour
{

    Vector3 position;
    float rotation;
    private void Update() 
    {
        if(!IsServer) return;
        
        position = transform.position;
        rotation = transform.rotation.y;
        PositioningClientRpc(position, rotation);
    }

    [ClientRpc]
    private void PositioningClientRpc(Vector3 position, float rotation)
    {
        if(IsOwner) return;
        
        transform.position = this.position;
        transform.rotation = Quaternion.Euler(0, this.rotation, 0);
    }
}
