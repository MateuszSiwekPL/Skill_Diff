using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TeamDeathMatch : NetworkBehaviour, IKillable
{
    [Header("Player Id")]
    public string team;
    public int id;
    private Vector3 spawnPosition;
    private void Start() 
    {
        spawnPosition = GameObject.Find(team + "_" + id.ToString()).transform.position;
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForFixedUpdate();
        SpawningServerRpc();
        Spawning();
    }

    [ServerRpc]
    public void SpawningServerRpc() => Spawning();
    public void Spawning()
    {
        transform.position = spawnPosition;
        transform.LookAt(new Vector3(0, transform.position.y, 0));
        
    }

    public void Kill()
    {
        SpawningServerRpc();
        Spawning();
    }
}
