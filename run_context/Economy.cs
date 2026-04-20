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
        get => _gold;
        set
        {
            if (value >= _gold)
            {
                PlayCoins();
            }

            _gold = value;
            gold_change?.Invoke(_gold);
        }
    }

    public int available_free_towers
    {
        get => _availableFreeTowers;
        set
        {
            _availableFreeTowers = value;
            available_free_towers_change?.Invoke(_availableFreeTowers);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }

        GD.PushError($"Not enough gold to spend: {amount} requested, {gold} available.");
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
