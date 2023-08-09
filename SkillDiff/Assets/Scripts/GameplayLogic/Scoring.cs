using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Scoring : NetworkBehaviour
{
    [Header("Scoreboard")]
    [SerializeField] private TextMeshProUGUI redScoreboard;
    [SerializeField] private TextMeshProUGUI blueScoreboard;

    [Header("Score Count")]
    public NetworkVariable<int> redScore = new NetworkVariable<int>(0);
    public NetworkVariable<int> blueScore = new NetworkVariable<int>(0);

    private void Awake() 
    {
        redScore.OnValueChanged += RedScoring;
        blueScore.OnValueChanged += BlueScoring;
    }

    private void RedScoring(int previous, int current) => redScoreboard.text = redScore.Value.ToString();
    private void BlueScoring(int previous, int current) => blueScoreboard.text = blueScore.Value.ToString();
}
