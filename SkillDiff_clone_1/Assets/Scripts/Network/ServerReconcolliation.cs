using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerReconcolliation : NetworkBehaviour
{
    [Header("Tick Logic")]
    float timeBetweetTicks = 1f;
    float tickRate = 30f;
   
    [SerializeField] Vector3[] positions = new Vector3[1024];
    int buffer = 1024;
    int tickCount;
    float timePassed;
    

    [Header("Position Reconcoliation")]
    Vector3 positionDifference;


    
   
    

    
    

  
   
    


    

}
