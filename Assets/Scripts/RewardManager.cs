using System;
using System.Collections.Generic;

public class CurrencyManager
{
    private Dictionary<CurrencyType, int> currencies = new Dictionary<CurrencyType, int>();

    public event Action<CurrencyType, int> OnCurrencyChanged;

    public CurrencyManager()
    {
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            currencies[type] = 0;
        }
    }

    public void AddReward(Reward reward)
    {
        if (currencies.ContainsKey(reward.type))
        {
            currencies[reward.type] += reward.amount;
        }
        else
        {
            currencies[reward.type] = reward.amount;
        }

        OnCurrencyChanged?.Invoke(reward.type, currencies[reward.type]);
    }

    public int GetCurrencyAmount(CurrencyType type)
    {
        return currencies.ContainsKey(type) ? currencies[type] : 0;
    }

    public Reward GenerateRandomReward(int minAmount, int maxAmount)
    {
        var rewardTypes = (CurrencyType[])Enum.GetValues(typeof(CurrencyType));
        
        CurrencyType randomType = rewardTypes[UnityEngine.Random.Range(0, rewardTypes.Length)];

        int randomAmount = UnityEngine.Random.Range(minAmount, maxAmount + 1);

        return new Reward(randomType, randomAmount);
    }  
}
