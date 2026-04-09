using Godot;

public static class ActionManager
{
    public enum ActionState
    {
        None,
        PlacingTower,
        UsingItem,
        TowerSelected,
    }

    private static ActionState _currentAction = ActionState.None;
    private static Callable _onActionCancel = default;

    public static ActionState CurrentAction => _currentAction;

    public static void StartAction(ActionState state, Callable cancelCallback = default)
    {
        if (_currentAction != ActionState.None)
        {
            EndAction();
        }

        _currentAction = state;
        _onActionCancel = cancelCallback;
    }

    public static void EndAction()
    {
        Callable callback = _onActionCancel;
        _onActionCancel = default;

        callback.Call();

        _currentAction = ActionState.None;
    }

    public static bool IsActionActive()
    {
        return _currentAction != ActionState.None;
    }
}
