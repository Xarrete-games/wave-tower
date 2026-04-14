using Godot;

public class Source
{
    public enum SourceType
    {
        RELIC,
        TOWER,
        CONSUMABLE,
        DEBUFF,
        GLOBAL,
    }

    public SourceType type;
    public string type_id;
    public Variant entity;
    public Source origin;

    public Source()
    {
    }

    public Source(SourceType p_type, string p_type_id, Variant p_entity = default, Source p_origin = null)
    {
        this.type = p_type;
        this.type_id = p_type_id;
        this.entity = p_entity;
        this.origin = p_origin;
    }

    public void setup(int p_type, string p_type_id)
    {
        this.type = (SourceType)p_type;
        this.type_id = p_type_id;
        this.entity = default;
        this.origin = null;
    }

    public void setup(int p_type, string p_type_id, Variant p_entity)
    {
        this.type = (SourceType)p_type;
        this.type_id = p_type_id;
        this.entity = p_entity;
        this.origin = null;
    }

    public void setup(int p_type, string p_type_id, Variant p_entity, Source p_origin)
    {
        this.type = (SourceType)p_type;
        this.type_id = p_type_id;
        this.entity = p_entity;
        this.origin = p_origin;
    }
}
