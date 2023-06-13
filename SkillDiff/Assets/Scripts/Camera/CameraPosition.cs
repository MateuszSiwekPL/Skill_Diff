using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [SerializeField] Transform player;

    private void Awake() {
        player = GameObject.Find("Player").transform;
    }
    
    void Update()
    {
        transform.position = player.position;
    }
}
