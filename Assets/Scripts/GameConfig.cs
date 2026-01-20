using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "TreasureHunt/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Configurations")]
    [Range(3, 12)]
    public int numberOfChests = 5;

    [Range(1, 10)]
    public int maxAttempts = 3;

    [Range(0.5f, 5f)]
    public float chestOpeningDuration = 2f;

    public int minRewardAmount = 10;
    public int maxRewardAmount = 100;
}
