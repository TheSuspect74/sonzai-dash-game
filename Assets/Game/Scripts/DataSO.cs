using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSO", menuName = "DataSO")]

[Serializable]
public class LeaderboardData
{
    public string name;
    public int score;
}

public class DataSO : ScriptableObject
{
    [Header("Player")] 
    public float playerSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldon;

    [Header("Enemy")] 
    public float enemySpeed;
    
    [Header("Game Config")] 
    public int collectibleAmount;
    public int startingLives;

    [Header("Dummy Leaderboard")] 
    public LeaderboardData[] dummyLeaderboardDatas;
}
