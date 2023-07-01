using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerReconcolliation : NetworkBehaviour
{
    [Header("Tick Logic")]
    float timeBetweetTicks = 1f;
    const float tickRate = 30f;
    [SerializeField] float tickDeltaTime;
    [SerializeField] int tickCount;
    int buffer = 1024;

    [Header("Position Reconcoliation")]
    [SerializeField] Vector3[] positions = new Vector3[1024];
    Vector3 positionDifference;

    

}
