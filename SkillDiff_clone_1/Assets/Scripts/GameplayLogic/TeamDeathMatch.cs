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
    Scoring scoring;
    
    private void Start() 
    {
        if (!IsServer) return;

        scoring = GameObject.Find("Score_Board").GetComponent<Scoring>();
        spawnPosition = GameObject.Find(team + "_" + id.ToString()).transform.position;
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForFixedUpdate();
        SpawningClientRpc(spawnPosition);
        Spawning();
    }

    [ClientRpc]
    public void SpawningClientRpc(Vector3 spawnPosition)
    {
        this.spawnPosition = spawnPosition;
        Spawning();
    }
    public void Spawning()
    {
        transform.position = spawnPosition;
        transform.LookAt(new Vector3(0, transform.position.y, 0));
        
    }

    public void Kill()
    {
        if(IsServer)
        {
            if(team == "Red")
            scoring.blueScore.Value++;
            else
            scoring.redScore.Value++;
                
                

            SpawningClientRpc(spawnPosition);
            Spawning();
        }
    }
}
