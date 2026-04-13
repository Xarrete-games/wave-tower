public class RelicsRewardsContext
{
    public int number_of_relics
    {
        get => this.NumberOfRelics;
        set => this.NumberOfRelics = value;
    }

    public int NumberOfRelics { get; set; }

    public RelicsRewardsContext(int numberOfRelics)
    {
        this.NumberOfRelics = numberOfRelics;
    }
}
