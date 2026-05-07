using Godot;

public static class TowerBuffFactory
{
    public static TowerBuff CreateFromId(string buffId, Source source, int value)
    {
        DataLoader dataLoader = DataLoader.Instance;
        if (dataLoader == null)
        {
            SceneTree tree = Engine.GetMainLoop() as SceneTree;
            dataLoader = tree?.Root?.GetNodeOrNull<DataLoader>("DataLoader");
        }

        BuffData buffData = dataLoader?.GetTowerBuffDataById(buffId);
        if (buffData == null)
        {
            GD.PushError($"[TowerBuffFactory] No buff data found for id: {buffId}");
            return null;
        }

        TowerBuff buff = buffData.CreateItem(source, value);
        if (buff == null)
        {
            GD.PushError($"[TowerBuffFactory] Could not create buff from data id: {buffId}");
            return null;
        }

        return buff;
    }
}
