using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [SerializeField] Transform player;
    
    void Update()
    {
        transform.position = player.position;
    }
}
