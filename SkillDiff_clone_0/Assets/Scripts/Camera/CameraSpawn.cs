using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraSpawn : NetworkBehaviour
{

    [SerializeField] GameObject cam;
    public override void OnNetworkSpawn()
    {
        if(IsLocalPlayer)
        {
            var playerCamera = Instantiate(cam);
            playerCamera.GetComponent<CameraLook>().player = transform;
            playerCamera.GetComponent<CameraPosition>().player = transform;
        }
    }
}
