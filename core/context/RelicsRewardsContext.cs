public class RelicsRewardsContext
{
    public int number_of_relics
    {
        get => NumberOfRelics;
        set => NumberOfRelics = value;
    }

    public int NumberOfRelics { get; set; }

    public RelicsRewardsContext(int numberOfRelics)
    {
        NumberOfRelics = numberOfRelics;
    }
}
