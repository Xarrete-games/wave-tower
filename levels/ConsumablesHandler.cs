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
        _runContext = GetNode<RunContext>("/root/RunContext");
        _gameState = GetNode<GameState>("/root/GameState");

        if (_runContext?.consumables_manager != null)
        {
            _runContext.consumables_manager.consumable_clicked += OnConsumableClicked;
        }

        if (_gameState != null)
        {
            _gameState.state_change += OnGameStateChanged;
        }

        if (_runContext?.towers_manager != null)
        {
            _runContext.towers_manager.tower_hovered += OnTowerHovered;
            _runContext.towers_manager.tower_unhovered += OnTowerUnhovered;
        }
    }

    public override void _ExitTree()
    {
        if (_runContext?.consumables_manager != null)
        {
            _runContext.consumables_manager.consumable_clicked -= OnConsumableClicked;
        }

        if (_gameState != null)
        {
            _gameState.state_change -= OnGameStateChanged;
        }

        if (_runContext?.towers_manager != null)
        {
            _runContext.towers_manager.tower_hovered -= OnTowerHovered;
            _runContext.towers_manager.tower_unhovered -= OnTowerUnhovered;
        }
    }

    public override void _Process(double delta)
    {
        if (_currentConsumable == null)
        {
            return;
        }

        ConsumableData data = _currentConsumable.data;
        if (data != null && data.TargetingType == (int)ConsumableTargeteable.TargetType.BLOCKED_TILE)
        {
            HandleBlockedTilePlacement();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_currentConsumable == null)
        {
            return;
        }

        if (UIUtilsStatic.IsLeftClickEvent(@event) && _isValidTarget)
        {
            UseConsumable();
        }
    }

    private void HandleBlockedTilePlacement()
    {
        if (composite_tile_map == null)
        {
            return;
        }

        bool isMouseOnBlockedTile = composite_tile_map.is_mouse_on_block_tile();
        if (isMouseOnBlockedTile)
        {
            ConsumableData data = _currentConsumable?.data;
            if (data != null)
            {
                Input.SetCustomMouseCursor(data.CursorIconUsed, Input.CursorShape.Arrow, CenterCursorOffset);
            }

            _isValidTarget = true;
            _currentTarget = composite_tile_map;
        }
        else
        {
            ConsumableData data = _currentConsumable?.data;
            if (data != null)
            {
                Input.SetCustomMouseCursor(data.CursorIcon, Input.CursorShape.Arrow, CenterCursorOffset);
            }

            InvalidateTarget();
        }
    }

    private void InvalidateTarget()
    {
        _isValidTarget = false;
        _currentTarget = null;
    }

    private void OnTowerHovered(Tower tower)
    {
        if (_currentConsumable == null)
        {
            return;
        }

        ConsumableData data = _currentConsumable.data;
        if (data == null || data.TargetingType != (int)ConsumableTargeteable.TargetType.TOWER)
        {
            return;
        }

        Input.SetCustomMouseCursor(data.CursorIconUsed, Input.CursorShape.Arrow, CenterCursorOffset);
        _isValidTarget = true;
        _currentTarget = tower;
    }

    private void OnTowerUnhovered(Tower tower)
    {
        if (_currentConsumable == null)
        {
            return;
        }

        ConsumableData data = _currentConsumable.data;
        if (data == null || data.TargetingType != (int)ConsumableTargeteable.TargetType.TOWER)
        {
            return;
        }

        if (ReferenceEquals(_currentTarget, tower))
        {
            Input.SetCustomMouseCursor(data.CursorIcon, Input.CursorShape.Arrow, CenterCursorOffset);
            InvalidateTarget();
        }
    }

    private void UseConsumable()
    {
        if (_currentConsumable == null)
        {
            return;
        }

        _currentConsumable.use(_currentTarget);
        CancelConsumable();

        ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
        actionManager.EndAction();
    }

    private void CancelConsumable()
    {
        _currentConsumable = null;
        InvalidateTarget();
        Input.SetCustomMouseCursor(DefaultCursor);
    }

    private void OnConsumableClicked(Consumable consumable)
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

        _currentConsumable = targeteable;
        InvalidateTarget();

        ConsumableData data = _currentConsumable.data;
        if (data != null)
        {
            Input.SetCustomMouseCursor(data.CursorIcon, Input.CursorShape.Arrow, CenterCursorOffset);
        }

        ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
        actionManager.StartAction(ActionManager.ActionState.UsingItem, CancelConsumable);
    }

    private void OnGameStateChanged(int newState)
    {
        if (newState != GameState.IN_GAME && _currentConsumable != null)
        {
            ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
            actionManager.EndAction();
        }
    }
}

