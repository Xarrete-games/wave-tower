using Godot;

public static class TowerBuffFactory
{
    public static TowerBuff create_from_id(string buff_id, Source source, int value)
    {
        DataLoader dataLoader = DataLoader.Instance;
        if (dataLoader == null)
        {
            SceneTree tree = Engine.GetMainLoop() as SceneTree;
            dataLoader = tree?.Root?.GetNodeOrNull<DataLoader>("DataLoader");
        }

        BuffData buffData = dataLoader?.GetTowerBuffDataById(buff_id);
        if (buffData == null)
        {
            GD.PushError($"[TowerBuffFactory] No buff data found for id: {buff_id}");
            return null;
        }

        TowerBuff buff = buffData.CreateItem(source, value);
        if (buff == null)
        {
            GD.PushError($"[TowerBuffFactory] Could not create buff from data id: {buff_id}");
            return null;
        }

        return buff;
    }
}
