using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PhysicsSimulate : NetworkBehaviour
{
    private void FixedUpdate() 
    {
        //if(IsOwner)
        //Simulate();
    }

    private void Simulate()
    {
        if(IsOwner)
        SimulateServerRpc();

        Physics.Simulate(Time.fixedDeltaTime);
    }

    [ServerRpc]
    private void SimulateServerRpc() => Simulate();
}
