using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerReconcolliation : MonoBehaviour
{
    [Header("Tick Logic")]
    float timeBetweetTicks;
    const int tickRate = 30;
    float tickDeltaTime;

    [Header("Position Reconcoliation")]
    Vector3 position;
    Vector3 velocity;
}
