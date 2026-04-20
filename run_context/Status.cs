using Godot;
using System;

public class Status
{
    public event Action<int> HealthChanged;
    public event Action<int> ArmorChanged;
    public event Action<int> MaxHealthChanged;
    public event Action PlayerDied;

    private int _maxHealth = 20;
    private int _health = 20;
    private int _armor;

    public int MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }

            MaxHealthChanged?.Invoke(_maxHealth);
        }
    }

    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Min(value, _maxHealth);
            HealthChanged?.Invoke(_health);
            if (_health <= 0)
            {
                var statusModel = new StatusModel
                {
                    MaxHealth = _maxHealth,
                    Health = _health,
                    Armor = _armor,
                };

                Hooks.OnBeforeDie(Hooks.GetListenersFromRuntime(), statusModel);

                bool maxHealthChanged = _maxHealth != statusModel.MaxHealth;
                bool healthChanged = _health != statusModel.Health;
                bool armorChanged = _armor != statusModel.Armor;

                _maxHealth = statusModel.MaxHealth;
                _health = statusModel.Health;
                _armor = statusModel.Armor;

                if (maxHealthChanged)
                {
                    MaxHealthChanged?.Invoke(_maxHealth);
                }

                if (healthChanged)
                {
                    HealthChanged?.Invoke(_health);
                }

                if (armorChanged)
                {
                    ArmorChanged?.Invoke(_armor);
                }

                if (_health <= 0)
                {
                    PlayerDied?.Invoke();
                }
            }
        }
    }

    public int Armor
    {
        get => _armor;
        set
        {
            _armor = value;
            ArmorChanged?.Invoke(_armor);
        }
    }

    public RunProgress Progress { get; private set; }
    public RelicsManager RelicsManager { get; private set; }

    public void Setup(RunProgress runProgress, RelicsManager relicsManager)
    {
        if (Progress != null)
        {
            Progress.current_wave_finished -= OnWaveFinished;
        }

        Progress = runProgress;
        RelicsManager = relicsManager;

        if (Progress != null)
        {
            Progress.current_wave_finished += OnWaveFinished;
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Health += amount;
    }

    public void AddArmor(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Armor += amount;
    }

    public void AddMaxHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        MaxHealth += amount;
        Health += amount;
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int remainingDamage = amount;
        bool armorBlockDamage = Armor >= remainingDamage;

        if (Armor > 0)
        {
            int absorbed = Mathf.Min(Armor, remainingDamage);
            Armor -= absorbed;
            remainingDamage -= absorbed;
            ArmorChanged?.Invoke(Armor);
        }

        if (remainingDamage > 0)
        {
            Health -= remainingDamage;
        }

        PlayDamageAudio(armorBlockDamage);
    }

    private void OnWaveFinished()
    {
        Armor = 0;
    }

    private void PlayDamageAudio(bool armorBlockDamage)
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return;
        }

        AudioManager audioManager = tree.Root.GetNodeOrNull<AudioManager>("/root/AudioManager");
        if (audioManager == null)
        {
            return;
        }

        if (armorBlockDamage)
        {
            audioManager.play_armor_block();
        }
        else
        {
            audioManager.play_player_hurt();
        }
    }
}

