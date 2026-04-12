using Godot;

[GlobalClass]
public partial class RelicsRewardsContext : RefCounted
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
