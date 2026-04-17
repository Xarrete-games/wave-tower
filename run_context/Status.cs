using Godot;
using System;

public class Status
{
    public event Action<int> health_change;
    public event Action<int> armor_change;
    public event Action<int> max_health_change;
    public event Action player_died;

    private int _maxHealth = 20;
    private int _health = 20;
    private int _armor;

    public int max_health
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }

            max_health_change?.Invoke(_maxHealth);
        }
    }

    public int health
    {
        get => _health;
        set
        {
            _health = Mathf.Min(value, _maxHealth);
            health_change?.Invoke(_health);
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
                    max_health_change?.Invoke(_maxHealth);
                }

                if (healthChanged)
                {
                    health_change?.Invoke(_health);
                }

                if (armorChanged)
                {
                    armor_change?.Invoke(_armor);
                }

                if (_health <= 0)
                {
                    player_died?.Invoke();
                }
            }
        }
    }

    public int armor
    {
        get => _armor;
        set
        {
            _armor = value;
            armor_change?.Invoke(_armor);
        }
    }

    public RunProgress progress { get; private set; }
    public RelicsManager relics_manager { get; private set; }

    public void setup(RunProgress runProgress, RelicsManager relicsManager)
    {
        if (progress != null)
        {
            progress.current_wave_finished -= OnWaveFinished;
        }

        progress = runProgress;
        relics_manager = relicsManager;

        if (progress != null)
        {
            progress.current_wave_finished += OnWaveFinished;
        }
    }

    public void heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        health += amount;
    }

    public void add_amor(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        armor += amount;
    }

    public void add_max_health(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        max_health += amount;
        health += amount;
    }

    public void apply_damage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int remainingDamage = amount;
        bool armorBlockDamage = armor >= remainingDamage;

        if (armor > 0)
        {
            int absorbed = Mathf.Min(armor, remainingDamage);
            armor -= absorbed;
            remainingDamage -= absorbed;
            armor_change?.Invoke(armor);
        }

        if (remainingDamage > 0)
        {
            health -= remainingDamage;
        }

        PlayDamageAudio(armorBlockDamage);
    }

    private void OnWaveFinished()
    {
        armor = 0;
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
