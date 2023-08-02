using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Rotating : NetworkBehaviour
{
    public float rotation;

    [ServerRpc]
    private void RotateServerRpc(float rot) => Rotate(rot);
    public void Rotate(float rot) 
    {
        if(IsOwner)
        RotateServerRpc(rot);
        
        transform.rotation = Quaternion.Euler(0, rot, 0);
    }
}
