public sealed class EconomyRuntime
{
    public bool IsSellActive { get; set; }
    public int Gold { get; set; } = 100;
    public int AvailableFreeTowers { get; set; }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }

        return false;
    }
}
