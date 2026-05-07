using Godot;
using System;

public class Economy
{
    public event Action<int> AvailableFreeTowersChanged;
    public event Action<int> GoldChanged;

    private int _gold = 10000;
    private int _availableFreeTowers;

    public bool IsSellActive { get; set; }

    public int Gold
    {
        get => _gold;
        set
        {
            if (value >= _gold)
            {
                PlayCoins();
            }

            _gold = value;
            GoldChanged?.Invoke(_gold);
        }
    }

    public int AvailableFreeTowers
    {
        get => _availableFreeTowers;
        set
        {
            _availableFreeTowers = value;
            AvailableFreeTowersChanged?.Invoke(_availableFreeTowers);
        }
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }

        GD.PushError($"Not enough gold to spend: {amount} requested, {Gold} available.");
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
        audioManager?.PlayCoins();
    }
}
