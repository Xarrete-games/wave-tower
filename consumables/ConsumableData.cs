using Godot;
using System;

[GlobalClass]
public partial class ConsumableData : BaseData
{
    [ExportGroup("Consumable")]
    [Export]
    public int consumable_type { get; set; }

    [Export]
    public int targeting_type { get; set; }

    [Export]
    public Texture2D cursor_icon { get; set; }

    [Export]
    public Texture2D cursor_icon_used { get; set; }

    [Export]
    public AudioStream use_sound { get; set; }

    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public Consumable create_consumable()
    {
        string idValue = (id ?? string.Empty).ToLowerInvariant();
        Consumable consumable = idValue switch
        {
            "first_aid" => new FirstAid(),
            "healing_potion" => new HealingPotion(),
            "poison_potion" => new PoisonPotion(),
            "second_skin" => new SecondSkin(),
            "magic_ring" => new MagicRing(),
            "long_shot" => new LongShot(),
            "foundation_breaker" => new FoundationBreaker(),
            "caffeine_potion" => new CaffeinePotion(),
            _ => CreateConsumableFromRuntimeScript(),
        };

        consumable?.init(this);
        return consumable;
    }

    private Consumable CreateConsumableFromRuntimeScript()
    {
        if (runtime_script == null)
        {
            GD.PushError($"[ConsumableData] Missing runtime_script for '{id}'");
            return null;
        }

        string scriptName = System.IO.Path.GetFileNameWithoutExtension(runtime_script.ResourcePath)?.ToLowerInvariant() ?? string.Empty;
        return scriptName switch
        {
            "firstaid" => new FirstAid(),
            "healingpotion" => new HealingPotion(),
            "poisonpotion" => new PoisonPotion(),
            "secondskin" => new SecondSkin(),
            "magicring" => new MagicRing(),
            "longshot" => new LongShot(),
            "foundationbreaker" => new FoundationBreaker(),
            "caffeinepotion" => new CaffeinePotion(),
            _ => null,
        };
    }

    public override Variant create_item()
    {
        // Consumables are now plain C# objects; callers should use create_consumable().
        return default;
    }
}
