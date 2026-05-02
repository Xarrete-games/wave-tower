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
        public PressureType Pressure = PressureType.MIXED;
        public List<EnemyData> Enemies = new();

        public int GetTotalWeight()
        {
            int total = 0;
            for (int i = 0; i < Enemies.Count; i++)
            {
                total += Enemies[i].Weight;
            }

            return total;
        }
    }

    private readonly WaveConfig _config;
    private readonly List<EnemyData> _enemyCatalog;

    public WaveComposer(WaveConfig config, List<EnemyData> enemyCatalog)
    {
        _config = config;
        _enemyCatalog = enemyCatalog ?? new List<EnemyData>();
    }

    public List<WaveGroup> ComposeWave(int waveNumber)
    {
        int totalBudget = CalculateBudget(waveNumber);
        var groups = new List<WaveGroup>();
        PressureType fullPressure = PickFullWavePressure(waveNumber);
        GD.Print($"[WaveComposer] Wave {waveNumber} mode: {PressureToString(fullPressure)}");

        if (IsBossWave(waveNumber))
        {
            WaveGroup bossGroup = CreateBossGroup(totalBudget, waveNumber);
            if (bossGroup != null)
            {
                groups.Add(bossGroup);
                totalBudget -= bossGroup.GetTotalWeight();
            }
        }

        if (fullPressure != PressureType.MIXED)
        {
            int numFullGroups = CalculateGroupCount(waveNumber);
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

                WaveGroup fullGroup = FillGroup(fullPressure, groupBudget, waveNumber, true);
                if (fullGroup.Enemies.Count > 0)
                {
                    groups.Add(fullGroup);
                }
            }

            GD.Print($"[WaveComposer] Wave {waveNumber} groups: {GroupsToLog(groups)}");
            return groups;
        }

        int numGroups = CalculateGroupCount(waveNumber);
        List<PressureType> pressures = PickUniquePressures(numGroups, waveNumber);

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

            WaveGroup group = FillGroup(pressures[i], groupBudget, waveNumber);
            if (group.Enemies.Count > 0)
            {
                groups.Add(group);
            }
        }

        GD.Print($"[WaveComposer] Wave {waveNumber} groups: {GroupsToLog(groups)}");
        return groups;
    }

    public int GetBudgetForWave(int waveNumber)
    {
        return CalculateBudget(waveNumber);
    }

    private int CalculateBudget(int waveNumber)
    {
        int linearBudget = _config.BaseBudget + ((waveNumber - 1) * _config.BudgetPerWave);
        int startWave = Mathf.Max(_config.ExponentialStartWave, 1);
        if (waveNumber <= startWave || _config.ExponentialGrowth <= 0.0f)
        {
            return linearBudget;
        }

        int growthSteps = waveNumber - startWave;
        float multiplier = Mathf.Pow(1.0f + _config.ExponentialGrowth, growthSteps);
        int scaledBudget = Mathf.RoundToInt(linearBudget * multiplier);
        return Mathf.Max(scaledBudget, linearBudget);
    }

    private bool IsBossWave(int waveNumber)
    {
        return waveNumber % _config.BossWaveEvery == 0;
    }

    private int CalculateGroupCount(int waveNumber)
    {
        if (waveNumber <= 2)
        {
            return 1;
        }

        if (waveNumber <= 4)
        {
            return (int)GD.RandRange(1, 2);
        }

        return (int)GD.RandRange(2, 3);
    }

    private List<PressureType> PickUniquePressures(int count, int waveNumber)
    {
        List<PressureType> pool = GetAvailablePressures(waveNumber);
        if (pool.Count == 0)
        {
            GD.PushError($"[WaveComposer] No pressure types available for wave {waveNumber}");
            return new List<PressureType> { PressureType.MIXED };
        }

        ShuffleList(pool);
        var result = new List<PressureType>();
        for (int i = 0; i < count; i++)
        {
            result.Add(pool[i % pool.Count]);
        }

        return result;
    }

    private PressureType PickFullWavePressure(int waveNumber)
    {
        WaveTypeChance chance = GetWaveTypeChance(waveNumber);
        if (chance == null)
        {
            GD.Print($"[WaveComposer] Wave {waveNumber} chances: none -> MIXED");
            return PressureType.MIXED;
        }

        float chanceSwarm = chance.ChanceFullSwarm;
        float chanceSpeed = chance.ChanceFullSpeed;
        float chanceTank = chance.ChanceFullTank;

        if (!IsValidWaveChanceValue(chanceSwarm))
        {
            GD.PushError($"[WaveComposer] Invalid ChanceFullSwarm in wave {waveNumber}. Expected 0..100, got {chanceSwarm}");
            return PressureType.MIXED;
        }

        if (!IsValidWaveChanceValue(chanceSpeed))
        {
            GD.PushError($"[WaveComposer] Invalid ChanceFullSpeed in wave {waveNumber}. Expected 0..100, got {chanceSpeed}");
            return PressureType.MIXED;
        }

        if (!IsValidWaveChanceValue(chanceTank))
        {
            GD.PushError($"[WaveComposer] Invalid ChanceFullTank in wave {waveNumber}. Expected 0..100, got {chanceTank}");
            return PressureType.MIXED;
        }

        float totalFull = chanceSwarm + chanceSpeed + chanceTank;
        if (totalFull > 100.0f)
        {
            GD.PushError($"[WaveComposer] Invalid wave chances in wave {waveNumber}. Sum must be <= 100, got {totalFull}");
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

        GD.Print($"[WaveComposer] Wave {waveNumber} chances S:{chanceSwarm} F:{chanceSpeed} T:{chanceTank} | roll: {roll:F2} | picked: {PressureToString(pressure)}");

        if (pressure == PressureType.MIXED)
        {
            return PressureType.MIXED;
        }

        if (!HasAvailableForPressure(pressure, waveNumber))
        {
            GD.PushError($"[WaveComposer] Wave {waveNumber} configured as FULL {PressureToString(pressure)}, but no enemies are available for that pressure in this wave");
            return PressureType.MIXED;
        }

        return pressure;
    }

    private WaveTypeChance GetWaveTypeChance(int waveNumber)
    {
        if (_config.WaveChances == null || _config.WaveChances.Length == 0)
        {
            return null;
        }

        int waveIndex = waveNumber - 1;
        if (waveIndex < 0 || waveIndex >= _config.WaveChances.Length)
        {
            return null;
        }

        return _config.WaveChances[waveIndex];
    }

    private bool IsValidWaveChanceValue(float value)
    {
        return value >= 0.0f && value <= 100.0f;
    }

    private List<PressureType> GetAvailablePressures(int waveNumber)
    {
        var pool = new List<PressureType>();
        if (HasAvailableForPressure(PressureType.SWARM, waveNumber))
        {
            pool.Add(PressureType.SWARM);
        }

        if (HasAvailableForPressure(PressureType.SPEED, waveNumber))
        {
            pool.Add(PressureType.SPEED);
        }

        if (HasAvailableForPressure(PressureType.TANK, waveNumber))
        {
            pool.Add(PressureType.TANK);
        }

        if (GetAllAvailable(waveNumber).Count > 0)
        {
            pool.Add(PressureType.MIXED);
        }

        return pool;
    }

    private bool HasAvailableForPressure(PressureType pressure, int waveNumber)
    {
        if (pressure == PressureType.MIXED)
        {
            return GetAllAvailable(waveNumber).Count > 0;
        }

        EnemyData.EnemyType enemyType = PressureToEnemyType(pressure);
        return GetAvailable(enemyType, waveNumber).Count > 0;
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

        var chunks = new List<string>();
        for (int i = 0; i < groups.Count; i++)
        {
            WaveGroup group = groups[i];
            chunks.Add($"#{i + 1}:{PressureToString(group.Pressure)}({group.Enemies.Count})");
        }

        return "[" + string.Join(", ", chunks) + "]";
    }

    private EnemyData.EnemyType PressureToEnemyType(PressureType pressure)
    {
        return pressure switch
        {
            PressureType.SWARM => EnemyData.EnemyType.SWARM,
            PressureType.SPEED => EnemyData.EnemyType.FAST,
            PressureType.TANK => EnemyData.EnemyType.TANK,
            _ => EnemyData.EnemyType.NORMAL,
        };
    }

    private WaveGroup CreateBossGroup(int availableBudget, int waveNumber)
    {
        List<EnemyData> bosses = GetAvailable(EnemyData.EnemyType.BOSS, waveNumber);
        if (bosses.Count == 0)
        {
            return null;
        }

        var affordableBosses = new List<EnemyData>();
        for (int i = 0; i < bosses.Count; i++)
        {
            EnemyData data = bosses[i];
            if (data.Weight <= availableBudget)
            {
                affordableBosses.Add(data);
            }
        }

        if (affordableBosses.Count == 0)
        {
            return null;
        }

        EnemyData boss = affordableBosses[(int)(GD.Randi() % (uint)affordableBosses.Count)];
        var group = new WaveGroup { Pressure = PressureType.TANK };
        group.Enemies.Add(boss);
        return group;
    }

    private WaveGroup FillGroup(PressureType pressure, int groupBudget, int waveNumber, bool fullOnly = false)
    {
        var group = new WaveGroup { Pressure = pressure };

        EnemyData.EnemyType primaryType = PressureToEnemyType(pressure);
        List<EnemyData> primaryCandidates = GetAvailable(primaryType, waveNumber);
        if (primaryCandidates.Count == 0)
        {
            primaryCandidates = GetAllAvailable(waveNumber);
        }

        int primarySpend = Mathf.RoundToInt(groupBudget * _config.PrimaryPressureRatio);
        if (fullOnly)
        {
            primarySpend = groupBudget;
        }

        groupBudget = FillBudget(group.Enemies, primaryCandidates, primarySpend, groupBudget);

        if (fullOnly)
        {
            ShuffleList(group.Enemies);
            return group;
        }

        List<EnemyData> mixedCandidates = GetAllAvailable(waveNumber);
        groupBudget = FillBudget(group.Enemies, mixedCandidates, groupBudget, groupBudget);

        ShuffleList(group.Enemies);
        return group;
    }

    private List<EnemyData> GetAvailable(EnemyData.EnemyType type, int waveNumber)
    {
        var result = new List<EnemyData>();
        for (int i = 0; i < _enemyCatalog.Count; i++)
        {
            EnemyData data = _enemyCatalog[i];
            if (data.Type == type && IsUnlocked(data, waveNumber))
            {
                result.Add(data);
            }
        }

        return result;
    }

    private List<EnemyData> GetAllAvailable(int waveNumber)
    {
        var result = new List<EnemyData>();
        for (int i = 0; i < _enemyCatalog.Count; i++)
        {
            EnemyData data = _enemyCatalog[i];
            if (data.Type != EnemyData.EnemyType.BOSS && IsUnlocked(data, waveNumber))
            {
                result.Add(data);
            }
        }

        return result;
    }

    private bool IsUnlocked(EnemyData data, int waveNumber)
    {
        return IsWaveAvailableForEnemy(data, waveNumber);
    }

    private bool IsWaveAvailableForEnemy(EnemyData data, int waveNumber)
    {
        if (data.AvailableWaves == null || data.AvailableWaves.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < data.AvailableWaves.Length; i++)
        {
            EnemyWaveRange waveRange = data.AvailableWaves[i];
            if (waveRange != null && IsWaveInRange(waveNumber, waveRange))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsWaveInRange(int waveNumber, EnemyWaveRange waveRange)
    {
        if (waveNumber < waveRange.InitialWave)
        {
            return false;
        }

        if (waveRange.FinalWave <= 0)
        {
            return true;
        }

        return waveNumber <= waveRange.FinalWave;
    }

    private int FillBudget(
        List<EnemyData> result,
        List<EnemyData> candidates,
        int targetSpend,
        int totalRemaining)
    {
        int spent = 0;
        while (spent < targetSpend && totalRemaining > 0)
        {
            var affordable = new List<EnemyData>();
            for (int i = 0; i < candidates.Count; i++)
            {
                EnemyData candidate = candidates[i];
                if (candidate.Weight <= totalRemaining)
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
            totalRemaining -= pick.Weight;
        }

        return totalRemaining;
    }

    private static void ShuffleList<T>(List<T> items)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int swapIndex = (int)(GD.Randi() % (uint)(i + 1));
            (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
        }
    }
}


