using Godot;
using System.Collections.Generic;

public partial class SanctuaryEventScript : EventScript
{
    public override List<EventOptionData> get_options()
    {
        var option1 = new EventOptionData("Take offering (+50 gold)", 0);
        var option2 = new EventOptionData("Pray (+10 health and 10 maximum health)", 1);
        return new List<EventOptionData> { option1, option2 };
    }

    public override void handle_response(object data)
    {
        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return;
        }

        int selectedOption = data is int intValue ? intValue : -1;
        switch (selectedOption)
        {
            case 0:
                runContext.economy.add_gold(50);
                break;
            case 1:
                runContext.status.add_MaxHealth(10);
                break;
        }
    }
}

