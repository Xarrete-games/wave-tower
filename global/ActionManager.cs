using Godot;

public partial class ActionManager : Node
{
    public enum ActionState
    {
        None,
        PlacingTower,
        UsingItem,
        TowerSelected,
    }

    public ActionState CurrentAction = ActionState.None;
    private Callable _onActionCancel = default;

    public void StartAction(ActionState state, Callable cancel_callback = default)
    {
        if (this.CurrentAction != ActionState.None)
        {
            this.EndAction();
        }

        this.CurrentAction = state;
        this._onActionCancel = cancel_callback;
    }

    public void EndAction()
    {
        Callable callback = this._onActionCancel;
        this._onActionCancel = default;

        if (!callback.Equals(default(Callable)))
        {
            try
            {
                callback.Call();
            }
            catch (System.ObjectDisposedException)
            {
                // Ignore stale UI callbacks when scene nodes were already freed.
            }
        }

        this.CurrentAction = ActionState.None;
    }

    public bool IsActionActive()
    {
        return this.CurrentAction != ActionState.None;
    }
}
