using Godot;

[GlobalClass]
public partial class ConsumablesHandler : Node
{
    private static readonly Vector2 CenterCursorOffset = new(24, 24);
    private static readonly Texture2D DefaultCursor = GD.Load<Texture2D>("res://assets/images/icons/mouse_02.png");

    [Export]
    public CompositeTileMap composite_tile_map;

    private ConsumableTargeteable _currentConsumable;
    private bool _isValidTarget;
    private object _currentTarget;

    private RunContext _runContext;
    private GameState _gameState;

    public override void _Ready()
    {
        this._runContext = GetNode<RunContext>("/root/RunContext");
        this._gameState = GetNode<GameState>("/root/GameState");

        if (this._runContext?.consumables_manager != null)
        {
            this._runContext.consumables_manager.consumable_clicked += this._on_consumable_clicked;
        }

        if (this._gameState != null)
        {
            this._gameState.state_change += this._on_game_state_changed;
        }

        if (this._runContext?.towers_manager != null)
        {
            this._runContext.towers_manager.tower_hovered += this._on_tower_hovered;
            this._runContext.towers_manager.tower_unhovered += this._on_tower_unhovered;
        }
    }

    public override void _ExitTree()
    {
        if (this._runContext?.consumables_manager != null)
        {
            this._runContext.consumables_manager.consumable_clicked -= this._on_consumable_clicked;
        }

        if (this._gameState != null)
        {
            this._gameState.state_change -= this._on_game_state_changed;
        }

        if (this._runContext?.towers_manager != null)
        {
            this._runContext.towers_manager.tower_hovered -= this._on_tower_hovered;
            this._runContext.towers_manager.tower_unhovered -= this._on_tower_unhovered;
        }
    }

    public override void _Process(double delta)
    {
        if (this._currentConsumable == null)
        {
            return;
        }

        ConsumableData data = this._currentConsumable.data;
        if (data != null && data.targeting_type == (int)ConsumableTargeteable.TargetType.BLOCKED_TILE)
        {
            this._handle_blocked_tile_placement();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (this._currentConsumable == null)
        {
            return;
        }

        if (UIUtilsStatic.IsLeftClickEvent(@event) && this._isValidTarget)
        {
            this._use_consumable();
        }
    }

    private void _handle_blocked_tile_placement()
    {
        if (this.composite_tile_map == null)
        {
            return;
        }

        bool isMouseOnBlockedTile = this.composite_tile_map.is_mouse_on_block_tile();
        if (isMouseOnBlockedTile)
        {
            ConsumableData data = this._currentConsumable?.data;
            if (data != null)
            {
                Input.SetCustomMouseCursor(data.cursor_icon_used, Input.CursorShape.Arrow, CenterCursorOffset);
            }

            this._isValidTarget = true;
            this._currentTarget = this.composite_tile_map;
        }
        else
        {
            ConsumableData data = this._currentConsumable?.data;
            if (data != null)
            {
                Input.SetCustomMouseCursor(data.cursor_icon, Input.CursorShape.Arrow, CenterCursorOffset);
            }

            this._invalidate_target();
        }
    }

    private void _invalidate_target()
    {
        this._isValidTarget = false;
        this._currentTarget = null;
    }

    private void _on_tower_hovered(Tower tower)
    {
        if (this._currentConsumable == null)
        {
            return;
        }

        ConsumableData data = this._currentConsumable.data;
        if (data == null || data.targeting_type != (int)ConsumableTargeteable.TargetType.TOWER)
        {
            return;
        }

        Input.SetCustomMouseCursor(data.cursor_icon_used, Input.CursorShape.Arrow, CenterCursorOffset);
        this._isValidTarget = true;
        this._currentTarget = tower;
    }

    private void _on_tower_unhovered(Tower tower)
    {
        if (this._currentConsumable == null)
        {
            return;
        }

        ConsumableData data = this._currentConsumable.data;
        if (data == null || data.targeting_type != (int)ConsumableTargeteable.TargetType.TOWER)
        {
            return;
        }

        if (ReferenceEquals(this._currentTarget, tower))
        {
            Input.SetCustomMouseCursor(data.cursor_icon, Input.CursorShape.Arrow, CenterCursorOffset);
            this._invalidate_target();
        }
    }

    private void _use_consumable()
    {
        if (this._currentConsumable == null)
        {
            return;
        }

        this._currentConsumable.use(this._currentTarget);
        this._cancel_consumable();

        ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
        actionManager.EndAction();
    }

    private void _cancel_consumable()
    {
        this._currentConsumable = null;
        this._invalidate_target();
        Input.SetCustomMouseCursor(DefaultCursor);
    }

    private void _on_consumable_clicked(Consumable consumable)
    {
        ConsumableTargeteable targeteable = consumable as ConsumableTargeteable;
        if (targeteable == null)
        {
            return;
        }

        if (!targeteable.requires_target())
        {
            return;
        }

        this._currentConsumable = targeteable;
        this._invalidate_target();

        ConsumableData data = this._currentConsumable.data;
        if (data != null)
        {
            Input.SetCustomMouseCursor(data.cursor_icon, Input.CursorShape.Arrow, CenterCursorOffset);
        }

        ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
        actionManager.StartAction(ActionManager.ActionState.UsingItem, Callable.From(this._cancel_consumable));
    }

    private void _on_game_state_changed(int newState)
    {
        if (newState != GameState.IN_GAME && this._currentConsumable != null)
        {
            ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
            actionManager.EndAction();
        }
    }
}
