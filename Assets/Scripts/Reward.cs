using System;

public enum CurrencyType{
    Coins, 
    Gems
}

[Serializable]
public struct Reward
{
    public CurrencyType type;
    public int amount;

    public Reward(CurrencyType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }

    public override string ToString()
    {
        return $"{amount} {type}";
    }
}
