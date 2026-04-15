using Godot;
using System;

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
    private Action _onActionCancel;

    public void StartAction(ActionState state, Action cancel_callback = null)
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
        Action callback = this._onActionCancel;
        this._onActionCancel = null;

        if (callback != null)
        {
            try
            {
                callback();
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
