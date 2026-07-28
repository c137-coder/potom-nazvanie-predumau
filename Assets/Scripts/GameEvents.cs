public readonly struct PlayerHealthChanged
{
    public readonly int Current;
    public readonly int Max;

    public PlayerHealthChanged(int current, int max)
    {
        Current = current;
        Max = max;
    }
}

public readonly struct PlayerAmmoChanged
{
    public readonly int Current;
    public readonly int Max;
    public readonly bool IsReloading;

    public PlayerAmmoChanged(int current, int max, bool isReloading)
    {
        Current = current;
        Max = max;
        IsReloading = isReloading;
    }
}

public readonly struct InventoryChanged
{
}
