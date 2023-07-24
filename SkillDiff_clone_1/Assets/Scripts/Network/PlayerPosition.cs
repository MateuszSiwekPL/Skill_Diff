using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPosition : NetworkBehaviour
{

    Vector3 position;
    float rotation;
    private void FixedUpdate() 
    {
        if(!IsServer) return;
        
        position = transform.position;
        rotation = transform.rotation.eulerAngles.y;
        PositioningClientRpc(position, rotation);
    }

    [ClientRpc]
    private void PositioningClientRpc(Vector3 position, float rotation)
    {
        if(IsOwner) return;
        
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, rotation, 0);
      
    }
}
