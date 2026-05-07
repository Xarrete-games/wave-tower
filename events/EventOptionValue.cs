using System;

public sealed class EventOptionValue
{
    public enum ValueKind
    {
        None,
        Int,
        Bool,
        String,
        RelicData,
        ConsumableData,
    }

    public static EventOptionValue Empty { get; } = new();

    public ValueKind Kind { get; private set; } = ValueKind.None;

    public int IntValue { get; private set; }

    public bool BoolValue { get; private set; }

    public string StringValue { get; private set; } = string.Empty;

    public RelicData RelicDataValue { get; private set; }

    public ConsumableData ConsumableDataValue { get; private set; }

    public static EventOptionValue FromInt(int value)
    {
        return new EventOptionValue { Kind = ValueKind.Int, IntValue = value };
    }

    public static EventOptionValue FromBool(bool value)
    {
        return new EventOptionValue { Kind = ValueKind.Bool, BoolValue = value };
    }

    public static EventOptionValue FromString(string value)
    {
        return new EventOptionValue { Kind = ValueKind.String, StringValue = value ?? string.Empty };
    }

    public static EventOptionValue FromRelicData(RelicData value)
    {
        return new EventOptionValue { Kind = ValueKind.RelicData, RelicDataValue = value };
    }

    public static EventOptionValue FromConsumableData(ConsumableData value)
    {
        return new EventOptionValue { Kind = ValueKind.ConsumableData, ConsumableDataValue = value };
    }

    public int RequireInt()
    {
        if (Kind != ValueKind.Int)
        {
            throw new InvalidOperationException($"Expected Int payload but got {Kind}.");
        }

        return IntValue;
    }

    public bool RequireBool()
    {
        if (Kind != ValueKind.Bool)
        {
            throw new InvalidOperationException($"Expected Bool payload but got {Kind}.");
        }

        return BoolValue;
    }

    public string RequireString()
    {
        if (Kind != ValueKind.String)
        {
            throw new InvalidOperationException($"Expected String payload but got {Kind}.");
        }

        return StringValue;
    }

    public RelicData RequireRelicData()
    {
        if (Kind != ValueKind.RelicData)
        {
            throw new InvalidOperationException($"Expected RelicData payload but got {Kind}.");
        }

        return RelicDataValue;
    }

    public ConsumableData RequireConsumableData()
    {
        if (Kind != ValueKind.ConsumableData)
        {
            throw new InvalidOperationException($"Expected ConsumableData payload but got {Kind}.");
        }

        return ConsumableDataValue;
    }
}
