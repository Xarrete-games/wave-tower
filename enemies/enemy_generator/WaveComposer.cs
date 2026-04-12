using Godot;

[GlobalClass]
public partial class WaveComposer : RefCounted
{
    public enum PressureType
    {
        SWARM,
        SPEED,
        TANK,
        MIXED,
    }

    [GlobalClass]
    public partial class WaveGroup : RefCounted
    {
        public PressureType pressure = PressureType.MIXED;
        public Godot.Collections.Array<EnemyData> enemies = new();

        public int get_total_weight()
        {
            int total = 0;
            for (int i = 0; i < this.enemies.Count; i++)
            {
                total += this.enemies[i].weight;
            }

            return total;
        }
    }

    private readonly WaveConfig _config;
    private readonly Godot.Collections.Array<EnemyData> _enemyCatalog;

    public WaveComposer(WaveConfig config, Godot.Collections.Array<EnemyData> enemy_catalog)
    {
        this._config = config;
        this._enemyCatalog = enemy_catalog ?? new Godot.Collections.Array<EnemyData>();
    }

    public Godot.Collections.Array<WaveGroup> compose_wave(int wave_number)
    {
        int totalBudget = this._calculate_budget(wave_number);
        var groups = new Godot.Collections.Array<WaveGroup>();
        PressureType fullPressure = this._pick_full_wave_pressure(wave_number);
        GD.Print($"[WaveComposer] Wave {wave_number} mode: {this._pressure_to_string(fullPressure)}");

        if (this._is_boss_wave(wave_number))
        {
            WaveGroup bossGroup = this._create_boss_group(totalBudget, wave_number);
            if (bossGroup != null)
            {
                groups.Add(bossGroup);
                totalBudget -= bossGroup.get_total_weight();
            }
        }

        if (fullPressure != PressureType.MIXED)
        {
            int numFullGroups = this._calculate_group_count(wave_number);
            int denom = Mathf.Max(numFullGroups, 1);
            int budgetPerFullGroup = totalBudget / denom;
            int fullRemainder = totalBudget - (budgetPerFullGroup * denom);

            for (int i = 0; i < numFullGroups; i++)
            {
                int groupBudget = budgetPerFullGroup + (i < fullRemainder ? 1 : 0);
                if (groupBudget <= 0)
                {
                    continue;
                }

                WaveGroup fullGroup = this._fill_group(fullPressure, groupBudget, wave_number, true);
                if (fullGroup.enemies.Count > 0)
                {
                    groups.Add(fullGroup);
                }
            }

            GD.Print($"[WaveComposer] Wave {wave_number} groups: {this._groups_to_log(groups)}");
            return groups;
        }

        int numGroups = this._calculate_group_count(wave_number);
        Godot.Collections.Array<PressureType> pressures = this._pick_unique_pressures(numGroups, wave_number);

        int safeNumGroups = Mathf.Max(numGroups, 1);
        int budgetPerGroup = totalBudget / safeNumGroups;
        int remainder = totalBudget - (budgetPerGroup * safeNumGroups);

        for (int i = 0; i < numGroups; i++)
        {
            int groupBudget = budgetPerGroup + (i < remainder ? 1 : 0);
            if (groupBudget <= 0)
            {
                continue;
            }

            WaveGroup group = this._fill_group(pressures[i], groupBudget, wave_number);
            if (group.enemies.Count > 0)
            {
                groups.Add(group);
            }
        }

        GD.Print($"[WaveComposer] Wave {wave_number} groups: {this._groups_to_log(groups)}");
        return groups;
    }

    public int get_budget_for_wave(int wave_number)
    {
        return this._calculate_budget(wave_number);
    }

    private int _calculate_budget(int wave_number)
    {
        int linearBudget = this._config.base_budget + ((wave_number - 1) * this._config.budget_per_wave);
        int startWave = Mathf.Max(this._config.exponential_start_wave, 1);
        if (wave_number <= startWave || this._config.exponential_growth <= 0.0f)
        {
            return linearBudget;
        }

        int growthSteps = wave_number - startWave;
        float multiplier = Mathf.Pow(1.0f + this._config.exponential_growth, growthSteps);
        int scaledBudget = Mathf.RoundToInt(linearBudget * multiplier);
        return Mathf.Max(scaledBudget, linearBudget);
    }

    private bool _is_boss_wave(int wave_number)
    {
        return wave_number % this._config.boss_wave_every == 0;
    }

    private int _calculate_group_count(int wave_number)
    {
        if (wave_number <= 2)
        {
            return 1;
        }

        if (wave_number <= 4)
        {
            return (int)GD.RandRange(1, 2);
        }

        return (int)GD.RandRange(2, 3);
    }

    private Godot.Collections.Array<PressureType> _pick_unique_pressures(int count, int wave_number)
    {
        Godot.Collections.Array<PressureType> pool = this._get_available_pressures(wave_number);
        if (pool.Count == 0)
        {
            GD.PushError($"[WaveComposer] No pressure types available for wave {wave_number}");
            return new Godot.Collections.Array<PressureType> { PressureType.MIXED };
        }

        pool.Shuffle();
        var result = new Godot.Collections.Array<PressureType>();
        for (int i = 0; i < count; i++)
        {
            result.Add(pool[i % pool.Count]);
        }

        return result;
    }

    private PressureType _pick_full_wave_pressure(int wave_number)
    {
        WaveTypeChance chance = this._get_wave_type_chance(wave_number);
        if (chance == null)
        {
            GD.Print($"[WaveComposer] Wave {wave_number} chances: none -> MIXED");
            return PressureType.MIXED;
        }

        float chanceSwarm = chance.chance_full_swarn;
        float chanceSpeed = chance.chance_full_speed;
        float chanceTank = chance.chance_full_tank;

        if (!this._is_valid_wave_chance_value(chanceSwarm))
        {
            GD.PushError($"[WaveComposer] Invalid chance_full_swarn in wave {wave_number}. Expected 0..100, got {chanceSwarm}");
            return PressureType.MIXED;
        }

        if (!this._is_valid_wave_chance_value(chanceSpeed))
        {
            GD.PushError($"[WaveComposer] Invalid chance_full_speed in wave {wave_number}. Expected 0..100, got {chanceSpeed}");
            return PressureType.MIXED;
        }

        if (!this._is_valid_wave_chance_value(chanceTank))
        {
            GD.PushError($"[WaveComposer] Invalid chance_full_tank in wave {wave_number}. Expected 0..100, got {chanceTank}");
            return PressureType.MIXED;
        }

        float totalFull = chanceSwarm + chanceSpeed + chanceTank;
        if (totalFull > 100.0f)
        {
            GD.PushError($"[WaveComposer] Invalid wave chances in wave {wave_number}. Sum must be <= 100, got {totalFull}");
            return PressureType.MIXED;
        }

        float roll = (float)GD.RandRange(0.0, 1.0) * 100.0f;
        PressureType pressure = PressureType.MIXED;
        if (roll < chanceSwarm)
        {
            pressure = PressureType.SWARM;
        }
        else if (roll < chanceSwarm + chanceSpeed)
        {
            pressure = PressureType.SPEED;
        }
        else if (roll < chanceSwarm + chanceSpeed + chanceTank)
        {
            pressure = PressureType.TANK;
        }

        GD.Print($"[WaveComposer] Wave {wave_number} chances S:{chanceSwarm} F:{chanceSpeed} T:{chanceTank} | roll: {roll:F2} | picked: {this._pressure_to_string(pressure)}");

        if (pressure == PressureType.MIXED)
        {
            return PressureType.MIXED;
        }

        if (!this._has_available_for_pressure(pressure, wave_number))
        {
            GD.PushError($"[WaveComposer] Wave {wave_number} configured as FULL {this._pressure_to_string(pressure)}, but no enemies are available for that pressure in this wave");
            return PressureType.MIXED;
        }

        return pressure;
    }

    private WaveTypeChance _get_wave_type_chance(int wave_number)
    {
        if (this._config.wave_chances == null || this._config.wave_chances.Count == 0)
        {
            return null;
        }

        int waveIndex = wave_number - 1;
        if (waveIndex < 0 || waveIndex >= this._config.wave_chances.Count)
        {
            return null;
        }

        return this._config.wave_chances[waveIndex];
    }

    private bool _is_valid_wave_chance_value(float value)
    {
        return value >= 0.0f && value <= 100.0f;
    }

    private Godot.Collections.Array<PressureType> _get_available_pressures(int wave_number)
    {
        var pool = new Godot.Collections.Array<PressureType>();
        if (this._has_available_for_pressure(PressureType.SWARM, wave_number))
        {
            pool.Add(PressureType.SWARM);
        }

        if (this._has_available_for_pressure(PressureType.SPEED, wave_number))
        {
            pool.Add(PressureType.SPEED);
        }

        if (this._has_available_for_pressure(PressureType.TANK, wave_number))
        {
            pool.Add(PressureType.TANK);
        }

        if (this._get_all_available(wave_number).Count > 0)
        {
            pool.Add(PressureType.MIXED);
        }

        return pool;
    }

    private bool _has_available_for_pressure(PressureType pressure, int wave_number)
    {
        if (pressure == PressureType.MIXED)
        {
            return this._get_all_available(wave_number).Count > 0;
        }

        EnemyData.Type enemyType = this._pressure_to_enemy_type(pressure);
        return this._get_available(enemyType, wave_number).Count > 0;
    }

    private string _pressure_to_string(PressureType pressure)
    {
        return pressure.ToString();
    }

    private string _groups_to_log(Godot.Collections.Array<WaveGroup> groups)
    {
        if (groups.Count == 0)
        {
            return "[]";
        }

        var chunks = new Godot.Collections.Array<string>();
        for (int i = 0; i < groups.Count; i++)
        {
            WaveGroup group = groups[i];
            chunks.Add($"#{i + 1}:{this._pressure_to_string(group.pressure)}({group.enemies.Count})");
        }

        return "[" + string.Join(", ", chunks) + "]";
    }

    private EnemyData.Type _pressure_to_enemy_type(PressureType pressure)
    {
        return pressure switch
        {
            PressureType.SWARM => EnemyData.Type.SWARM,
            PressureType.SPEED => EnemyData.Type.FAST,
            PressureType.TANK => EnemyData.Type.TANK,
            _ => EnemyData.Type.NORMAL,
        };
    }

    private WaveGroup _create_boss_group(int available_budget, int wave_number)
    {
        Godot.Collections.Array<EnemyData> bosses = this._get_available(EnemyData.Type.BOSS, wave_number);
        if (bosses.Count == 0)
        {
            return null;
        }

        var affordableBosses = new Godot.Collections.Array<EnemyData>();
        for (int i = 0; i < bosses.Count; i++)
        {
            EnemyData data = bosses[i];
            if (data.weight <= available_budget)
            {
                affordableBosses.Add(data);
            }
        }

        if (affordableBosses.Count == 0)
        {
            return null;
        }

        EnemyData boss = affordableBosses[(int)(GD.Randi() % (uint)affordableBosses.Count)];
        var group = new WaveGroup { pressure = PressureType.TANK };
        group.enemies.Add(boss);
        return group;
    }

    private WaveGroup _fill_group(PressureType pressure, int group_budget, int wave_number, bool full_only = false)
    {
        var group = new WaveGroup { pressure = pressure };

        EnemyData.Type primaryType = this._pressure_to_enemy_type(pressure);
        Godot.Collections.Array<EnemyData> primaryCandidates = this._get_available(primaryType, wave_number);
        if (primaryCandidates.Count == 0)
        {
            primaryCandidates = this._get_all_available(wave_number);
        }

        int primarySpend = Mathf.RoundToInt(group_budget * this._config.primary_pressure_ratio);
        if (full_only)
        {
            primarySpend = group_budget;
        }

        group_budget = this._fill_budget(group.enemies, primaryCandidates, primarySpend, group_budget);

        if (full_only)
        {
            group.enemies.Shuffle();
            return group;
        }

        Godot.Collections.Array<EnemyData> mixedCandidates = this._get_all_available(wave_number);
        group_budget = this._fill_budget(group.enemies, mixedCandidates, group_budget, group_budget);

        group.enemies.Shuffle();
        return group;
    }

    private Godot.Collections.Array<EnemyData> _get_available(EnemyData.Type type, int wave_number)
    {
        var result = new Godot.Collections.Array<EnemyData>();
        for (int i = 0; i < this._enemyCatalog.Count; i++)
        {
            EnemyData data = this._enemyCatalog[i];
            if (data.type == type && this._is_unlocked(data, wave_number))
            {
                result.Add(data);
            }
        }

        return result;
    }

    private Godot.Collections.Array<EnemyData> _get_all_available(int wave_number)
    {
        var result = new Godot.Collections.Array<EnemyData>();
        for (int i = 0; i < this._enemyCatalog.Count; i++)
        {
            EnemyData data = this._enemyCatalog[i];
            if (data.type != EnemyData.Type.BOSS && this._is_unlocked(data, wave_number))
            {
                result.Add(data);
            }
        }

        return result;
    }

    private bool _is_unlocked(EnemyData data, int wave_number)
    {
        return this._is_wave_available_for_enemy(data, wave_number);
    }

    private bool _is_wave_available_for_enemy(EnemyData data, int wave_number)
    {
        if (data.available_waves == null || data.available_waves.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < data.available_waves.Count; i++)
        {
            EnemyWaveRange waveRange = data.available_waves[i];
            if (waveRange != null && this._is_wave_in_range(wave_number, waveRange))
            {
                return true;
            }
        }

        return false;
    }

    private bool _is_wave_in_range(int wave_number, EnemyWaveRange wave_range)
    {
        if (wave_number < wave_range.initial_wave)
        {
            return false;
        }

        if (wave_range.final_wave <= 0)
        {
            return true;
        }

        return wave_number <= wave_range.final_wave;
    }

    private int _fill_budget(
        Godot.Collections.Array<EnemyData> result,
        Godot.Collections.Array<EnemyData> candidates,
        int target_spend,
        int total_remaining)
    {
        int spent = 0;
        while (spent < target_spend && total_remaining > 0)
        {
            var affordable = new Godot.Collections.Array<EnemyData>();
            for (int i = 0; i < candidates.Count; i++)
            {
                EnemyData candidate = candidates[i];
                if (candidate.weight <= total_remaining)
                {
                    affordable.Add(candidate);
                }
            }

            if (affordable.Count == 0)
            {
                break;
            }

            EnemyData pick = affordable[(int)(GD.Randi() % (uint)affordable.Count)];
            result.Add(pick);
            spent += pick.weight;
            total_remaining -= pick.weight;
        }

        return total_remaining;
    }
}
