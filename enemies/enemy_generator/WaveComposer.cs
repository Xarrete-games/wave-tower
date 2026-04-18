using Godot;
using System.Collections.Generic;

public class WaveComposer
{
    public enum PressureType
    {
        SWARM,
        SPEED,
        TANK,
        MIXED,
    }

    public class WaveGroup
    {
        public PressureType pressure = PressureType.MIXED;
        public Godot.Collections.Array<EnemyData> enemies = new();

        public int get_total_weight()
        {
            int total = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                total += enemies[i].Weight;
            }

            return total;
        }
    }

    private readonly WaveConfig _config;
    private readonly Godot.Collections.Array<EnemyData> _enemyCatalog;

    public WaveComposer(WaveConfig config, Godot.Collections.Array<EnemyData> enemy_catalog)
    {
        _config = config;
        _enemyCatalog = enemy_catalog ?? new Godot.Collections.Array<EnemyData>();
    }

    public List<WaveGroup> compose_wave(int wave_number)
    {
        int totalBudget = CalculateBudget(wave_number);
        var groups = new List<WaveGroup>();
        PressureType fullPressure = PickFullWavePressure(wave_number);
        GD.Print($"[WaveComposer] Wave {wave_number} mode: {PressureToString(fullPressure)}");

        if (IsBossWave(wave_number))
        {
            WaveGroup bossGroup = CreateBossGroup(totalBudget, wave_number);
            if (bossGroup != null)
            {
                groups.Add(bossGroup);
                totalBudget -= bossGroup.get_total_weight();
            }
        }

        if (fullPressure != PressureType.MIXED)
        {
            int numFullGroups = CalculateGroupCount(wave_number);
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

                WaveGroup fullGroup = FillGroup(fullPressure, groupBudget, wave_number, true);
                if (fullGroup.enemies.Count > 0)
                {
                    groups.Add(fullGroup);
                }
            }

            GD.Print($"[WaveComposer] Wave {wave_number} groups: {GroupsToLog(groups)}");
            return groups;
        }

        int numGroups = CalculateGroupCount(wave_number);
        Godot.Collections.Array<PressureType> pressures = _pick_unique_pressures(numGroups, wave_number);

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

            WaveGroup group = FillGroup(pressures[i], groupBudget, wave_number);
            if (group.enemies.Count > 0)
            {
                groups.Add(group);
            }
        }

        GD.Print($"[WaveComposer] Wave {wave_number} groups: {GroupsToLog(groups)}");
        return groups;
    }

    public int get_budget_for_wave(int wave_number)
    {
        return CalculateBudget(wave_number);
    }

    private int CalculateBudget(int wave_number)
    {
        int linearBudget = _config.BaseBudget + ((wave_number - 1) * _config.BudgetPerWave);
        int startWave = Mathf.Max(_config.ExponentialStartWave, 1);
        if (wave_number <= startWave || _config.ExponentialGrowth <= 0.0f)
        {
            return linearBudget;
        }

        int growthSteps = wave_number - startWave;
        float multiplier = Mathf.Pow(1.0f + _config.ExponentialGrowth, growthSteps);
        int scaledBudget = Mathf.RoundToInt(linearBudget * multiplier);
        return Mathf.Max(scaledBudget, linearBudget);
    }

    private bool IsBossWave(int wave_number)
    {
        return wave_number % _config.BossWaveEvery == 0;
    }

    private int CalculateGroupCount(int wave_number)
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
        Godot.Collections.Array<PressureType> pool = _get_available_pressures(wave_number);
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

    private PressureType PickFullWavePressure(int wave_number)
    {
        WaveTypeChance chance = GetWaveTypeChance(wave_number);
        if (chance == null)
        {
            GD.Print($"[WaveComposer] Wave {wave_number} chances: none -> MIXED");
            return PressureType.MIXED;
        }

        float chanceSwarm = chance.ChanceFullSwarm;
        float chanceSpeed = chance.ChanceFullSpeed;
        float chanceTank = chance.ChanceFullTank;

        if (!IsValidWaveChanceValue(chanceSwarm))
        {
            GD.PushError($"[WaveComposer] Invalid ChanceFullSwarm in wave {wave_number}. Expected 0..100, got {chanceSwarm}");
            return PressureType.MIXED;
        }

        if (!IsValidWaveChanceValue(chanceSpeed))
        {
            GD.PushError($"[WaveComposer] Invalid ChanceFullSpeed in wave {wave_number}. Expected 0..100, got {chanceSpeed}");
            return PressureType.MIXED;
        }

        if (!IsValidWaveChanceValue(chanceTank))
        {
            GD.PushError($"[WaveComposer] Invalid ChanceFullTank in wave {wave_number}. Expected 0..100, got {chanceTank}");
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

        GD.Print($"[WaveComposer] Wave {wave_number} chances S:{chanceSwarm} F:{chanceSpeed} T:{chanceTank} | roll: {roll:F2} | picked: {PressureToString(pressure)}");

        if (pressure == PressureType.MIXED)
        {
            return PressureType.MIXED;
        }

        if (!HasAvailableForPressure(pressure, wave_number))
        {
            GD.PushError($"[WaveComposer] Wave {wave_number} configured as FULL {PressureToString(pressure)}, but no enemies are available for that pressure in this wave");
            return PressureType.MIXED;
        }

        return pressure;
    }

    private WaveTypeChance GetWaveTypeChance(int wave_number)
    {
        if (_config.WaveChances == null || _config.WaveChances.Count == 0)
        {
            return null;
        }

        int waveIndex = wave_number - 1;
        if (waveIndex < 0 || waveIndex >= _config.WaveChances.Count)
        {
            return null;
        }

        return _config.WaveChances[waveIndex];
    }

    private bool IsValidWaveChanceValue(float value)
    {
        return value >= 0.0f && value <= 100.0f;
    }

    private Godot.Collections.Array<PressureType> _get_available_pressures(int wave_number)
    {
        var pool = new Godot.Collections.Array<PressureType>();
        if (HasAvailableForPressure(PressureType.SWARM, wave_number))
        {
            pool.Add(PressureType.SWARM);
        }

        if (HasAvailableForPressure(PressureType.SPEED, wave_number))
        {
            pool.Add(PressureType.SPEED);
        }

        if (HasAvailableForPressure(PressureType.TANK, wave_number))
        {
            pool.Add(PressureType.TANK);
        }

        if (_get_all_available(wave_number).Count > 0)
        {
            pool.Add(PressureType.MIXED);
        }

        return pool;
    }

    private bool HasAvailableForPressure(PressureType pressure, int wave_number)
    {
        if (pressure == PressureType.MIXED)
        {
            return _get_all_available(wave_number).Count > 0;
        }

        EnemyData.EnemyType enemyType = _pressure_to_enemy_type(pressure);
        return _get_available(enemyType, wave_number).Count > 0;
    }

    private string PressureToString(PressureType pressure)
    {
        return pressure.ToString();
    }

    private string GroupsToLog(List<WaveGroup> groups)
    {
        if (groups.Count == 0)
        {
            return "[]";
        }

        var chunks = new Godot.Collections.Array<string>();
        for (int i = 0; i < groups.Count; i++)
        {
            WaveGroup group = groups[i];
            chunks.Add($"#{i + 1}:{PressureToString(group.pressure)}({group.enemies.Count})");
        }

        return "[" + string.Join(", ", chunks) + "]";
    }

    private EnemyData.EnemyType _pressure_to_enemy_type(PressureType pressure)
    {
        return pressure switch
        {
            PressureType.SWARM => EnemyData.EnemyType.SWARM,
            PressureType.SPEED => EnemyData.EnemyType.FAST,
            PressureType.TANK => EnemyData.EnemyType.TANK,
            _ => EnemyData.EnemyType.NORMAL,
        };
    }

    private WaveGroup CreateBossGroup(int available_budget, int wave_number)
    {
        Godot.Collections.Array<EnemyData> bosses = _get_available(EnemyData.EnemyType.BOSS, wave_number);
        if (bosses.Count == 0)
        {
            return null;
        }

        var affordableBosses = new Godot.Collections.Array<EnemyData>();
        for (int i = 0; i < bosses.Count; i++)
        {
            EnemyData data = bosses[i];
            if (data.Weight <= available_budget)
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

    private WaveGroup FillGroup(PressureType pressure, int group_budget, int wave_number, bool full_only = false)
    {
        var group = new WaveGroup { pressure = pressure };

        EnemyData.EnemyType primaryType = _pressure_to_enemy_type(pressure);
        Godot.Collections.Array<EnemyData> primaryCandidates = _get_available(primaryType, wave_number);
        if (primaryCandidates.Count == 0)
        {
            primaryCandidates = _get_all_available(wave_number);
        }

        int primarySpend = Mathf.RoundToInt(group_budget * _config.PrimaryPressureRatio);
        if (full_only)
        {
            primarySpend = group_budget;
        }

        group_budget = FillBudget(group.enemies, primaryCandidates, primarySpend, group_budget);

        if (full_only)
        {
            group.enemies.Shuffle();
            return group;
        }

        Godot.Collections.Array<EnemyData> mixedCandidates = _get_all_available(wave_number);
        group_budget = FillBudget(group.enemies, mixedCandidates, group_budget, group_budget);

        group.enemies.Shuffle();
        return group;
    }

    private Godot.Collections.Array<EnemyData> _get_available(EnemyData.EnemyType type, int wave_number)
    {
        var result = new Godot.Collections.Array<EnemyData>();
        for (int i = 0; i < _enemyCatalog.Count; i++)
        {
            EnemyData data = _enemyCatalog[i];
            if (data.Type == type && IsUnlocked(data, wave_number))
            {
                result.Add(data);
            }
        }

        return result;
    }

    private Godot.Collections.Array<EnemyData> _get_all_available(int wave_number)
    {
        var result = new Godot.Collections.Array<EnemyData>();
        for (int i = 0; i < _enemyCatalog.Count; i++)
        {
            EnemyData data = _enemyCatalog[i];
            if (data.Type != EnemyData.EnemyType.BOSS && IsUnlocked(data, wave_number))
            {
                result.Add(data);
            }
        }

        return result;
    }

    private bool IsUnlocked(EnemyData data, int wave_number)
    {
        return IsWaveAvailableForEnemy(data, wave_number);
    }

    private bool IsWaveAvailableForEnemy(EnemyData data, int wave_number)
    {
        if (data.AvailableWaves == null || data.AvailableWaves.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < data.AvailableWaves.Count; i++)
        {
            EnemyWaveRange waveRange = data.AvailableWaves[i];
            if (waveRange != null && IsWaveInRange(wave_number, waveRange))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsWaveInRange(int wave_number, EnemyWaveRange wave_range)
    {
        if (wave_number < wave_range.InitialWave)
        {
            return false;
        }

        if (wave_range.FinalWave <= 0)
        {
            return true;
        }

        return wave_number <= wave_range.FinalWave;
    }

    private int FillBudget(
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
                if (candidate.Weight <= total_remaining)
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
            spent += pick.Weight;
            total_remaining -= pick.Weight;
        }

        return total_remaining;
    }
}


