using Godot;
using System.Collections.Generic;

public partial class BloodPactScript : EventScript
{
    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        var option1 = new EventOptionData("Sacrifice 15 of your health to gain a powerful relic.", EventOptionValue.FromBool(true));
        var option2 = new EventOptionData("Walk away unharmed.", EventOptionValue.FromBool(false));
        return new List<EventOptionData> { option1, option2 };
    }

    public override void HandleResponse(EventOptionValue data)
    {
        bool accepted = data.RequireBool();
        if (!accepted)
        {
            return;
        }

        RunContext runContext = GetRunContext();
        runContext.Status.ApplyDamage(15);

        List<RelicData> allRelics = DataLoaderAccess.GetNotUsedRelicsTyped();
        if (allRelics.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)allRelics.Count);
        RelicData relicData = allRelics[randomIndex];
        Relic relic = relicData.CreateItem();
        if (relic != null)
        {
            runContext.RelicsManager.AddRelic(relic);
        }
    }
}
