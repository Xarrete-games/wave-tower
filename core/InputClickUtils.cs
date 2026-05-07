using Godot;

public static class InputClickUtils
{
    public static bool IsLeftClickReleased(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventMouseButton mouseButton)
        {
            return false;
        }

        return mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed;
    }

    public static bool IsRightClickReleased(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventMouseButton mouseButton)
        {
            return false;
        }

        return mouseButton.ButtonIndex == MouseButton.Right && !mouseButton.Pressed;
    }
}