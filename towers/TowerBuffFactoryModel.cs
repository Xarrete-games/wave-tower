public static class TowerBuffFactoryModel
{
    public static TowerBuffModel CreateFromId(string buffId, string sourceId, int value)
    {
        if (string.IsNullOrEmpty(buffId) || string.IsNullOrEmpty(sourceId))
        {
            return null;
        }

        return new TowerBuffModel(buffId, sourceId, value);
    }
}
