using Godot;

[GlobalClass]
public partial class TowerBuffFactory : RefCounted
{
    public static Variant create_from_id(string buff_id, Variant source, int value)
    {
        DataLoader dataLoader = DataLoader.Instance;
        if (dataLoader == null)
        {
            SceneTree tree = Engine.GetMainLoop() as SceneTree;
            dataLoader = tree?.Root?.GetNodeOrNull<DataLoader>("DataLoader");
        }

        Variant buffDataVar = dataLoader?.get_tower_buff_data_by_id(buff_id) ?? default;
        BuffData buffData = buffDataVar.As<BuffData>();
        if (buffData == null)
        {
            GD.PushError($"[TowerBuffFactory] No buff data found for id: {buff_id}");
            return default;
        }

        Variant buff = buffData.create_item(source, value);
        if (buff.VariantType == Variant.Type.Nil)
        {
            GD.PushError($"[TowerBuffFactory] Could not create buff from data id: {buff_id}");
            return default;
        }

        return buff;
    }
}
