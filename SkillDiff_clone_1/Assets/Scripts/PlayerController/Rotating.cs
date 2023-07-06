using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Rotating : NetworkBehaviour
{
    public float rotation;


    private void FixedUpdate() 
    {
        if(!IsOwner) return;

        RotateServerRpc(rotation);
        Rotate(rotation);
    }

    [ServerRpc]
    private void RotateServerRpc(float rot) => Rotate(rot);
    private void Rotate(float rot) => transform.rotation = Quaternion.Euler(0, rot, 0);
}
