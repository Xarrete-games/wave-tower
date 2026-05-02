using Godot;
using System;

[GlobalClass]
public partial class ConsumableData : BaseData
{
    [ExportGroup("Consumable")]
    [Export]
    public int ConsumableType { get; set; }

    [Export]
    public int TargetingType { get; set; }

    [Export]
    public Texture2D CursorIcon { get; set; }

    [Export]
    public Texture2D CursorIconUsed { get; set; }

    [Export]
    public AudioStream UseSound { get; set; }

    [ExportGroup("Script")]
    [Export]
    public Script RuntimeScript { get; set; }

    public Consumable CreateConsumable()
    {
        string idValue = (Id ?? string.Empty).ToLowerInvariant();
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

        consumable?.Init(this);
        return consumable;
    }

    private Consumable CreateConsumableFromRuntimeScript()
    {
        if (RuntimeScript == null)
        {
            GD.PushError($"[ConsumableData] Missing RuntimeScript for '{Id}'");
            return null;
        }

        string scriptName = System.IO.Path.GetFileNameWithoutExtension(RuntimeScript.ResourcePath)?.ToLowerInvariant() ?? string.Empty;
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

}
