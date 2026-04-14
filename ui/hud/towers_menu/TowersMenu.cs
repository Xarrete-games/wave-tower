using Godot;
using System.Collections.Generic;

public partial class TowersMenu : Control
{
    [Export]
    public Control buttons_container;

    [Export]
    public Control tower_hint;

    [Export]
    public PackedScene tower_button_scene;

    private GodotObject _buttonInHover;
    private readonly Dictionary<string, Node> _buttons = new();
    private TowersManager _towersManager;

    public override void _Ready()
    {
        if (this.tower_hint != null)
        {
            this.tower_hint.Visible = false;
        }

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._towersManager = runContext?.towers_manager;
        if (this._towersManager != null)
        {
            this._towersManager.tower_card_amount_change += this.OnTowerCardAdded;
        }
        this.InitButtonCards();
    }

    public override void _ExitTree()
    {
        if (this._towersManager != null)
        {
            this._towersManager.tower_card_amount_change -= this.OnTowerCardAdded;
            this._towersManager = null;
        }
    }

    private void InitButtonCards()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        foreach (KeyValuePair<string, int> pair in runContext.towers_manager.tower_cards_amount)
        {
            Variant towerConfiguration = runContext.towers_manager.get_tower_configuration_by_id(pair.Key);
            this.OnTowerCardAdded(towerConfiguration, pair.Value);
        }
    }

    private void OnTowerCardAdded(Variant towerData, int amount)
    {
        string towerId = this.GetTowerId(towerData);
        if (string.IsNullOrEmpty(towerId))
        {
            return;
        }

        if (amount == 0)
        {
            if (this._buttons.TryGetValue(towerId, out Node existingButton))
            {
                existingButton.QueueFree();
                this._buttons.Remove(towerId);
            }

            return;
        }

        if (!this._buttons.TryGetValue(towerId, out Node buttonNode))
        {
            buttonNode = this.tower_button_scene.Instantiate();
            this.buttons_container.AddChild(buttonNode);
            buttonNode.Set("tower_data", towerData);
            buttonNode.Set("amount", amount);

            buttonNode.Connect("tower_button_pressed", Callable.From<Variant, int>(this.OnTowerButtonPressed));
            buttonNode.Connect("hover", Callable.From<Variant>(this.OnTowerButtonHover));
            buttonNode.Connect("unhover", Callable.From<Variant>(this.OnTowerButtonUnhover));

            this._buttons[towerId] = buttonNode;
            return;
        }

        buttonNode.Set("amount", amount);
    }

    private string GetTowerId(Variant towerData)
    {
        GodotObject towerObj = towerData.AsGodotObject();
        if (towerObj == null)
        {
            return string.Empty;
        }

        GodotObject data = towerObj.Get("data").AsGodotObject();
        if (data == null)
        {
            return string.Empty;
        }

        return (string)data.Get("id");
    }

    private void OnTowerButtonPressed(Variant towerData, int price)
    {
        ClickEventsBus.EmitTowerBuildButtonPressed(towerData, price);
    }

    private void OnTowerButtonHover(Variant towerButton)
    {
        GodotObject towerButtonObj = towerButton.AsGodotObject();
        if (towerButtonObj == null || this.tower_hint == null)
        {
            return;
        }

        this._buttonInHover = towerButtonObj;

        GodotObject towerData = towerButtonObj.Get("tower_data").AsGodotObject();
        GodotObject data = towerData?.Get("data").AsGodotObject();
        if (data == null)
        {
            return;
        }

        this.tower_hint.Call("set_stats", data);

        Rect2 rect = towerButtonObj.Call("get_global_rect").As<Rect2>();
        this.tower_hint.GlobalPosition = new Vector2(
            rect.Position.X + rect.Size.X * 0.25f - this.tower_hint.Size.X * 0.5f,
            this.tower_hint.GlobalPosition.Y
        );
        this.tower_hint.Visible = true;
    }

    private void OnTowerButtonUnhover(Variant towerButton)
    {
        GodotObject towerButtonObj = towerButton.AsGodotObject();
        if (towerButtonObj == null || this.tower_hint == null)
        {
            return;
        }

        if (this._buttonInHover == towerButtonObj)
        {
            this._buttonInHover = null;
            this.tower_hint.Visible = false;
        }
    }
}