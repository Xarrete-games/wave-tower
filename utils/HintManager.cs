using System.Collections.Generic;
using Godot;

public static class HintManagerStatic
{
    public enum PositionHint
    {
        RIGHT,
        BOTTOM,
    }

    private static readonly PackedScene HintScene = GD.Load<PackedScene>("uid://dfcxnivp7sm3h");
    private static readonly Vector2 Offset = Vector2.Zero;
    private static readonly Dictionary<Control, Node> Hints = new();

    public static void ShowHint(Node context, Control parent, string text, string title = "", PositionHint pos = PositionHint.BOTTOM)
    {
        if (context == null || parent == null || HintScene == null)
        {
            return;
        }

        RemoveHint(parent);

        Hint hint = HintScene.Instantiate<Hint>();
        context.GetTree().Root.AddChild(hint);
        hint.set_text(text);
        hint.set_title(title);

        Vector2 basePos = parent.GlobalPosition + GetOffset(parent, pos);
        hint.set_position(basePos);
        Hints[parent] = hint;

        if (!IsOnLeftSide(context, parent) && pos == PositionHint.BOTTOM)
        {
            Vector2 viewportSize = context.GetViewport().GetVisibleRect().Size;
            float hintWidth = hint.get_size().X;
            Vector2 newPos = hint.get_position();
            newPos.X = parent.GlobalPosition.X + parent.Size.X - hintWidth;
            newPos.X = Mathf.Clamp(newPos.X, 0.0f, viewportSize.X - hintWidth);
            newPos.Y = parent.GlobalPosition.Y + parent.Size.Y;
            hint.set_position(newPos);
        }
    }

    public static void RemoveHint(Control parent)
    {
        if (parent == null)
        {
            return;
        }

        if (Hints.TryGetValue(parent, out Node hint))
        {
            hint.QueueFree();
            Hints.Remove(parent);
        }
    }

    public static Vector2 GetOffset(Control parent, PositionHint pos)
    {
        if (parent == null)
        {
            return Offset;
        }

        if (pos == PositionHint.RIGHT)
        {
            return Offset + new Vector2(parent.Size.X, 0.0f);
        }

        return Offset + new Vector2(0.0f, parent.Size.Y + 20.0f);
    }

    public static bool IsOnLeftSide(Node context, Control parent)
    {
        if (context == null || parent == null)
        {
            return true;
        }

        float halfX = context.GetViewport().GetVisibleRect().Size.X * 0.5f;
        return parent.GlobalPosition.X < halfX;
    }
}