using Godot;

[GlobalClass]
public partial class Economy : RefCounted
{
    [Signal]
    public delegate void available_free_towers_changeEventHandler(int amount);

    [Signal]
    public delegate void gold_changeEventHandler(int amount);

    private int _gold = 100;
    private int _availableFreeTowers;

    public bool is_sell_active { get; set; }

    public int gold
    {
        get => this._gold;
        set
        {
            if (value >= this._gold)
            {
                this.PlayCoins();
            }

            this._gold = value;
            this.EmitSignal(SignalName.gold_change, this._gold);
        }
    }

    public int available_free_towers
    {
        get => this._availableFreeTowers;
        set
        {
            this._availableFreeTowers = value;
            this.EmitSignal(SignalName.available_free_towers_change, this._availableFreeTowers);
        }
    }

    public void add_gold(int amount)
    {
        this.gold += amount;
    }

    public bool spend_gold(int amount)
    {
        if (this.gold >= amount)
        {
            this.gold -= amount;
            return true;
        }

        GD.PushError($"Not enough gold to spend: {amount} requested, {this.gold} available.");
        return false;
    }

    private void PlayCoins()
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return;
        }

        Node audioManager = tree.Root.GetNodeOrNull<Node>("/root/AudioManager");
        audioManager?.Call("play_coins");
    }
}
