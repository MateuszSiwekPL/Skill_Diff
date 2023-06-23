using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerRotation : NetworkBehaviour
{   
    [ServerRpc]
    public void RotationServerRpc(float rotY)
    {
        transform.rotation = Quaternion.Euler(0, rotY, 0);
    }
}
