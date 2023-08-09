using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerReconsiliation : NetworkBehaviour
{

    [Header("Server Reconciliation")]
    [SerializeField] int tick = 0;
    [SerializeField] Vector3[] positions = new Vector3[1024];
    int buffer = 1024;
    Rigidbody rb;


    private void Awake() => rb = gameObject.GetComponent<Rigidbody>();

    
    public void FixedUpdate()
    {
        if(IsOwner)
        {
            ClientPrediction();
        }
    }

    [ServerRpc]
    private void ClientPredictionServerRpc(int clientTick) 
    {
        tick = clientTick;
        ClientPrediction();
    }

    private void ClientPrediction()
    {
        if(IsOwner)
        ClientPredictionServerRpc(tick);


        Physics.Simulate(Time.fixedDeltaTime);

        if(IsOwner)
        {
            positions[tick % buffer] = transform.position;
            tick ++;
        }
        
        if(IsServer)
        {
            PositionCorrectionClientRpc(transform.position, tick, rb.velocity, rb.angularVelocity);
            positions[tick % buffer] = transform.position;
        }
        Debug.Log(tick.ToString());
    }

    [ClientRpc]
    public void PositionCorrectionClientRpc(Vector3 serverPosition, int serverTick, Vector3 velocity, Vector3 aVelocity)
    {
        if(!IsOwner) return;
        
        Vector3 correction = serverPosition - positions[serverTick % buffer];
        if (correction.magnitude > 0.00000001)
        {
            transform.position = serverPosition;
            rb.velocity = velocity;
            rb.angularVelocity = aVelocity;
            Debug.Log(correction.ToString());
            tick = serverTick;
        }
    }
}
