using System;

public class WalletSystem : Singleton<WalletSystem>
{
    private int _coins;
    public int Coins => _coins;

    public event Action<int> OnCoinsChanged;

    public bool TrySpend(int amount)
    {
        if (Coins < amount) return false;

        _coins -= amount;
        OnCoinsChanged?.Invoke(Coins);
        return true;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        _coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }
}

