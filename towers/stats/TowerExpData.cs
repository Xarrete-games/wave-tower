public class TowerExpData
{
    public int Level = 1;
    public int CurrentExp = 0;
    public int ExpForNextLevel = 0;

    public TowerExpData()
    {
    }

    public TowerExpData(int newLevel, int newCurrentExp, int newExpForNextLevel)
    {
        Level = newLevel;
        CurrentExp = newCurrentExp;
        ExpForNextLevel = newExpForNextLevel;
    }
}
