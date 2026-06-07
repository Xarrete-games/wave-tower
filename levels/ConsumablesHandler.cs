using Godot;

[GlobalClass]
public partial class ConsumablesHandler : Node
{
    private static readonly Vector2 CenterCursorOffset = new(24, 24);
    private static readonly Texture2D DefaultCursor = GD.Load<Texture2D>("res://assets/images/icons/mouse_02.png");

    [Export]
    public CompositeTileMap CompositeTileMap;

    private ConsumableTargeteable _currentConsumable;
    private bool _isValidTarget;
    private Node _currentTarget;

    private RunContext _runContext;
    private GameState _gameState;

    public override void _Ready()
    {
        _runContext = RunContext.Instance;
        _gameState = GetNode<GameState>("/root/GameState");

        _runContext.ConsumablesManager.ConsumableClicked += OnConsumableClicked;

        if (_gameState != null)
        {
            _gameState.StateChanged += OnGameStateChanged;
        }

        _runContext.TowersManager.TowerHovered += OnTowerHovered;
        _runContext.TowersManager.TowerUnhovered += OnTowerUnhovered;
    }

    public override void _ExitTree()
    {
        if (_runContext != null)
        {
            _runContext.ConsumablesManager.ConsumableClicked -= OnConsumableClicked;
        }

        if (_gameState != null)
        {
            _gameState.StateChanged -= OnGameStateChanged;
        }

        if (_runContext != null)
        {
            _runContext.TowersManager.TowerHovered -= OnTowerHovered;
            _runContext.TowersManager.TowerUnhovered -= OnTowerUnhovered;
        }
    }

    public override void _Process(double delta)
    {
        if (_currentConsumable == null)
        {
            return;
        }

        ConsumableData data = _currentConsumable.Data;
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
        if (CompositeTileMap == null)
        {
            return;
        }

        bool isMouseOnBlockedTile = CompositeTileMap.IsMouseOnBlockTile();
        if (isMouseOnBlockedTile)
        {
            ConsumableData data = _currentConsumable?.Data;
            if (data != null)
            {
                Input.SetCustomMouseCursor(data.CursorIconUsed, Input.CursorShape.Arrow, CenterCursorOffset);
            }

            _isValidTarget = true;
            _currentTarget = CompositeTileMap;
        }
        else
        {
            ConsumableData data = _currentConsumable?.Data;
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

    private void OnTowerHovered(TowerNode tower)
    {
        if (_currentConsumable == null)
        {
            return;
        }

        ConsumableData data = _currentConsumable.Data;
        if (data == null || data.TargetingType != (int)ConsumableTargeteable.TargetType.TOWER)
        {
            return;
        }

        Input.SetCustomMouseCursor(data.CursorIconUsed, Input.CursorShape.Arrow, CenterCursorOffset);
        _isValidTarget = true;
        _currentTarget = tower;
    }

    private void OnTowerUnhovered(TowerNode tower)
    {
        if (_currentConsumable == null)
        {
            return;
        }

        ConsumableData data = _currentConsumable.Data;
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

        _currentConsumable.Use(_currentTarget);
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

        if (!targeteable.RequiresTarget())
        {
            return;
        }

        _currentConsumable = targeteable;
        InvalidateTarget();

        ConsumableData data = _currentConsumable.Data;
        if (data != null)
        {
            Input.SetCustomMouseCursor(data.CursorIcon, Input.CursorShape.Arrow, CenterCursorOffset);
        }

        ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
        actionManager.StartAction(ActionManager.ActionState.UsingItem, CancelConsumable);
    }

    private void OnGameStateChanged(int newState)
    {
        if (newState != GameState.InGame && _currentConsumable != null)
        {
            ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
            actionManager.EndAction();
        }
    }
}

