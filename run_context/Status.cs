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
        get => this._maxHealth;
        set
        {
            this._maxHealth = value;
            if (this._health > this._maxHealth)
            {
                this._health = this._maxHealth;
            }

            this.max_health_change?.Invoke(this._maxHealth);
        }
    }

    public int health
    {
        get => this._health;
        set
        {
            this._health = Mathf.Min(value, this._maxHealth);
            this.health_change?.Invoke(this._health);
            if (this._health <= 0)
            {
                var statusModel = new StatusModel
                {
                    MaxHealth = this._maxHealth,
                    Health = this._health,
                    Armor = this._armor,
                };

                Hooks.OnBeforeDie(Hooks.GetListenersFromRuntime(), statusModel);

                bool maxHealthChanged = this._maxHealth != statusModel.MaxHealth;
                bool healthChanged = this._health != statusModel.Health;
                bool armorChanged = this._armor != statusModel.Armor;

                this._maxHealth = statusModel.MaxHealth;
                this._health = statusModel.Health;
                this._armor = statusModel.Armor;

                if (maxHealthChanged)
                {
                    this.max_health_change?.Invoke(this._maxHealth);
                }

                if (healthChanged)
                {
                    this.health_change?.Invoke(this._health);
                }

                if (armorChanged)
                {
                    this.armor_change?.Invoke(this._armor);
                }

                if (this._health <= 0)
                {
                    this.player_died?.Invoke();
                }
            }
        }
    }

    public int armor
    {
        get => this._armor;
        set
        {
            this._armor = value;
            this.armor_change?.Invoke(this._armor);
        }
    }

    public RunProgress progress { get; private set; }
    public RelicsManager relics_manager { get; private set; }

    public void setup(RunProgress p_progress, RelicsManager p_relics_manager)
    {
        if (this.progress != null)
        {
            this.progress.current_wave_finished -= this._on_wave_finished;
        }

        this.progress = p_progress;
        this.relics_manager = p_relics_manager;

        if (this.progress != null)
        {
            this.progress.current_wave_finished += this._on_wave_finished;
        }
    }

    public void heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        this.health += amount;
    }

    public void add_amor(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        this.armor += amount;
    }

    public void add_max_health(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        this.max_health += amount;
        this.health += amount;
    }

    public void apply_damage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int remainingDamage = amount;
        bool armorBlockDamage = this.armor >= remainingDamage;

        if (this.armor > 0)
        {
            int absorbed = Mathf.Min(this.armor, remainingDamage);
            this.armor -= absorbed;
            remainingDamage -= absorbed;
            this.armor_change?.Invoke(this.armor);
        }

        if (remainingDamage > 0)
        {
            this.health -= remainingDamage;
        }

        this.PlayDamageAudio(armorBlockDamage);
    }

    private void _on_wave_finished()
    {
        this.armor = 0;
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
