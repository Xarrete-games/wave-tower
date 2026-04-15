using Godot;
using System;

public class Economy
{
    public event Action<int> available_free_towers_change;
    public event Action<int> gold_change;

    private int _gold = 10000;
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
            this.gold_change?.Invoke(this._gold);
        }
    }

    public int available_free_towers
    {
        get => this._availableFreeTowers;
        set
        {
            this._availableFreeTowers = value;
            this.available_free_towers_change?.Invoke(this._availableFreeTowers);
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

        AudioManager audioManager = tree.Root.GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.play_coins();
    }
}
