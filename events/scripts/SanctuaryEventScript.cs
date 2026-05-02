using Godot;
using System.Collections.Generic;

public partial class SanctuaryEventScript : EventScript
{
    public override IReadOnlyList<EventOptionData> GetOptions()
    {
        var option1 = new EventOptionData("Take offering (+50 gold)", EventOptionValue.FromInt(0));
        var option2 = new EventOptionData("Pray (+10 health and 10 maximum health)", EventOptionValue.FromInt(1));
        return new List<EventOptionData> { option1, option2 };
    }

    public override void HandleResponse(EventOptionValue data)
    {
        RunContext runContext = GetRunContext();
        int selectedOption = data.RequireInt();
        switch (selectedOption)
        {
            case 0:
                runContext.Economy.AddGold(50);
                break;
            case 1:
                runContext.Status.AddMaxHealth(10);
                break;
        }
    }
}

