class_name WaveComposer extends RefCounted
## Decides WHAT enemies to spawn for a given wave.
##
## Uses the wave number, a WaveConfig, and the full enemy catalog to:
##  1. Calculate the wave's total budget.
##  2. Decide how many pressure groups the wave will have.
##  3. Pick a different pressure type for each group.
##  4. Fill each group's budget with matching enemies from the catalog.
##
## The result is an Array[WaveGroup] — each group represents a distinct
## batch of enemies with a coherent pressure identity. A single wave
## can mix swarm, speed and tank groups to create varied gameplay.

# ---------------------------------------------------------
# INNER CLASS — WaveGroup
# ---------------------------------------------------------

## A self-contained batch of enemies that share a pressure identity.
## The WaveSpawner processes groups sequentially, spawning each
## group's enemies before moving to the next.
class WaveGroup extends RefCounted:
	## The pressure flavour of this group (for debug / UI).
	var pressure: PressureType = PressureType.MIXED
	## Ordered list of enemies to spawn in this group.
	var enemies: Array[EnemyData] = []

	## Sum of all enemy weights in this group.
	func get_total_weight() -> int:
		var total: int = 0
		for e in enemies:
			total += e.weight
		return total

# ---------------------------------------------------------
# PRESSURE TYPES
# ---------------------------------------------------------

## High-level wave flavour that shapes the enemy composition.
enum PressureType {
	SWARM, ## Many cheap enemies — overwhelm by numbers
	SPEED, ## Fast enemies — punish slow reactions
	TANK,  ## Few tanky enemies — require sustained damage
	MIXED, ## Balanced mix of all unlocked types
}

# ---------------------------------------------------------
# INTERNAL STATE
# ---------------------------------------------------------

var _config: WaveConfig
var _enemy_catalog: Array[EnemyData]

# ---------------------------------------------------------
# LIFECYCLE
# ---------------------------------------------------------

func _init(config: WaveConfig, enemy_catalog: Array[EnemyData]) -> void:
	_config = config
	_enemy_catalog = enemy_catalog

# ---------------------------------------------------------
# PUBLIC API
# ---------------------------------------------------------

## Composes the wave for [param wave_number].
## Returns an Array[WaveGroup] — one or more pressure groups
## that the spawner will process sequentially.
func compose_wave(wave_number: int) -> Array[WaveGroup]:
	var total_budget: int = _calculate_budget(wave_number)
	var groups: Array[WaveGroup] = []
	var full_pressure: PressureType = _pick_full_wave_pressure(wave_number)
	print("[WaveComposer] Wave %d mode: %s" % [wave_number, _pressure_to_string(full_pressure)])

	# --- Boss group (if this is a boss wave) ---
	if _is_boss_wave(wave_number):
		var boss_group: WaveGroup = _create_boss_group(total_budget, wave_number)
		if boss_group != null:
			groups.append(boss_group)
			total_budget -= boss_group.get_total_weight()

	# --- Regular pressure groups ---
	if full_pressure != PressureType.MIXED:
		var full_group: WaveGroup = _fill_group(full_pressure, total_budget, wave_number, true)
		if full_group.enemies.size() > 0:
			groups.append(full_group)
		print("[WaveComposer] Wave %d groups: %s" % [wave_number, _groups_to_log(groups)])
		return groups

	var num_groups: int = _calculate_group_count(wave_number)
	var pressures: Array[PressureType] = _pick_unique_pressures(num_groups, wave_number)

	# Distribute remaining budget across groups (roughly even, remainder to first)
	@warning_ignore("integer_division")
	var budget_per_group: int = total_budget / maxi(num_groups, 1)
	var remainder: int = total_budget - budget_per_group * maxi(num_groups, 1)

	for i in range(num_groups):
		var group_budget: int = budget_per_group + (1 if i < remainder else 0)
		if group_budget <= 0:
			continue
		var group: WaveGroup = _fill_group(pressures[i], group_budget, wave_number)
		if group.enemies.size() > 0:
			groups.append(group)

	print("[WaveComposer] Wave %d groups: %s" % [wave_number, _groups_to_log(groups)])
	return groups

# ---------------------------------------------------------
# BUDGET HELPERS
# ---------------------------------------------------------

## Total budget available for the given wave.
func _calculate_budget(wave_number: int) -> int:
	return _config.base_budget + (wave_number - 1) * _config.budget_per_wave

## Whether [param wave_number] should include a boss enemy.
func _is_boss_wave(wave_number: int) -> bool:
	return (
		wave_number % _config.boss_wave_every == 0
	)

# ---------------------------------------------------------
# GROUP COUNT
# ---------------------------------------------------------

## How many pressure groups this wave should have.
## Early waves are simple (1 group), later waves combine up to 3 pressures.
func _calculate_group_count(wave_number: int) -> int:
	if wave_number <= 2:
		return 1
	elif wave_number <= 4:
		return randi_range(1, 2)
	else:
		return randi_range(2, 3)

# ---------------------------------------------------------
# PRESSURE SELECTION
# ---------------------------------------------------------

## Picks [param count] distinct pressures unlocked for [param wave_number].
## If fewer types are unlocked than requested, some may repeat.
func _pick_unique_pressures(count: int, wave_number: int) -> Array[PressureType]:
	var pool: Array[PressureType] = _get_available_pressures(wave_number)
	if pool.is_empty():
		push_error("[WaveComposer] No pressure types available for wave %d" % wave_number)
		return [PressureType.MIXED]

	pool.shuffle()

	var result: Array[PressureType] = []
	for i in range(count):
		result.append(pool[i % pool.size()])
	return result

func _pick_full_wave_pressure(wave_number: int) -> PressureType:
	var chance: WaveTypeChance = _get_wave_type_chance(wave_number)
	if chance == null:
		print("[WaveComposer] Wave %d chances: none -> MIXED" % wave_number)
		return PressureType.MIXED

	var chance_swarm: float = chance.chance_full_swarn
	var chance_speed: float = chance.chance_full_speed
	var chance_tank: float = chance.chance_full_tank

	if not _is_valid_wave_chance_value(chance_swarm):
		push_error("[WaveComposer] Invalid chance_full_swarn in wave %d. Expected 0..100, got %s" % [wave_number, chance_swarm])
		return PressureType.MIXED
	if not _is_valid_wave_chance_value(chance_speed):
		push_error("[WaveComposer] Invalid chance_full_speed in wave %d. Expected 0..100, got %s" % [wave_number, chance_speed])
		return PressureType.MIXED
	if not _is_valid_wave_chance_value(chance_tank):
		push_error("[WaveComposer] Invalid chance_full_tank in wave %d. Expected 0..100, got %s" % [wave_number, chance_tank])
		return PressureType.MIXED

	var total_full: float = chance_swarm + chance_speed + chance_tank
	if total_full > 100.0:
		push_error("[WaveComposer] Invalid wave chances in wave %d. Sum must be <= 100, got %s" % [wave_number, total_full])
		return PressureType.MIXED

	var roll: float = randf() * 100.0
	var pressure: PressureType = PressureType.MIXED

	if roll < chance_swarm:
		pressure = PressureType.SWARM
	elif roll < chance_swarm + chance_speed:
		pressure = PressureType.SPEED
	elif roll < chance_swarm + chance_speed + chance_tank:
		pressure = PressureType.TANK

	print(
		"[WaveComposer] Wave %d chances S:%s F:%s T:%s | roll: %.2f | picked: %s" % [
			wave_number,
			chance_swarm,
			chance_speed,
			chance_tank,
			roll,
			_pressure_to_string(pressure)
		]
	)

	if pressure == PressureType.MIXED:
		return PressureType.MIXED

	if not _has_available_for_pressure(pressure, wave_number):
		push_error(
			"[WaveComposer] Wave %d configured as FULL %s, but no enemies are available for that pressure in this wave" % [
				wave_number,
				_pressure_to_string(pressure)
			]
		)
		return PressureType.MIXED

	return pressure

func _get_wave_type_chance(wave_number: int) -> WaveTypeChance:
	if _config.wave_chances.is_empty():
		return null

	var wave_index: int = wave_number - 1
	if wave_index < 0 or wave_index >= _config.wave_chances.size():
		return null

	return _config.wave_chances[wave_index]

func _is_valid_wave_chance_value(value: float) -> bool:
	return value >= 0.0 and value <= 100.0

func _get_available_pressures(wave_number: int) -> Array[PressureType]:
	var pool: Array[PressureType] = []
	if _has_available_for_pressure(PressureType.SWARM, wave_number):
		pool.append(PressureType.SWARM)
	if _has_available_for_pressure(PressureType.SPEED, wave_number):
		pool.append(PressureType.SPEED)
	if _has_available_for_pressure(PressureType.TANK, wave_number):
		pool.append(PressureType.TANK)

	if _get_all_available(wave_number).size() > 0:
		pool.append(PressureType.MIXED)

	return pool

func _has_available_for_pressure(pressure: PressureType, wave_number: int) -> bool:
	if pressure == PressureType.MIXED:
		return _get_all_available(wave_number).size() > 0

	var enemy_type: EnemyData.Type = _pressure_to_enemy_type(pressure)
	return _get_available(enemy_type, wave_number).size() > 0

func _pressure_to_string(pressure: PressureType) -> String:
	match pressure:
		PressureType.SWARM:
			return "SWARM"
		PressureType.SPEED:
			return "SPEED"
		PressureType.TANK:
			return "TANK"
		PressureType.MIXED:
			return "MIXED"
		_:
			return "UNKNOWN"

func _groups_to_log(groups: Array[WaveGroup]) -> String:
	if groups.is_empty():
		return "[]"

	var chunks: Array[String] = []
	for i in range(groups.size()):
		var group: WaveGroup = groups[i]
		chunks.append("#%d:%s(%d)" % [i + 1, _pressure_to_string(group.pressure), group.enemies.size()])

	return "[" + ", ".join(chunks) + "]"

## Maps a PressureType to the EnemyData.Type used for catalog filtering.
func _pressure_to_enemy_type(pressure: PressureType) -> EnemyData.Type:
	match pressure:
		PressureType.SWARM:
			return EnemyData.Type.SWARM
		PressureType.SPEED:
			return EnemyData.Type.FAST
		PressureType.TANK:
			return EnemyData.Type.TANK
		_:
			return EnemyData.Type.NORMAL

# ---------------------------------------------------------
# BOSS GROUP
# ---------------------------------------------------------

## Creates a dedicated boss group consuming part of the total budget.
func _create_boss_group(available_budget: int, wave_number: int) -> WaveGroup:
	var bosses: Array[EnemyData] = _get_available(EnemyData.Type.BOSS, wave_number)
	if bosses.size() == 0:
		return null

	# Keep boss progression deterministic:
	# always pick the highest unlocked boss tier that is affordable.
	var affordable_bosses: Array[EnemyData] = bosses.filter(func(data: EnemyData) -> bool:
		return data.weight <= available_budget
	)
	if affordable_bosses.size() == 0:
		return null

	# Boss progression is defined by available_waves ranges.
	# Pick any currently available and affordable boss.
	var boss: EnemyData = affordable_bosses[randi() % affordable_bosses.size()]

	var group: WaveGroup = WaveGroup.new()
	group.pressure = PressureType.TANK
	group.enemies.append(boss)
	return group

# ---------------------------------------------------------
# GROUP FILLING
# ---------------------------------------------------------

## Fills a single group with enemies matching [param pressure] until
## [param group_budget] is exhausted.
func _fill_group(pressure: PressureType, group_budget: int, wave_number: int, full_only: bool = false) -> WaveGroup:
	var group: WaveGroup = WaveGroup.new()
	group.pressure = pressure

	var primary_type: EnemyData.Type = _pressure_to_enemy_type(pressure)
	var primary_candidates: Array[EnemyData] = _get_available(primary_type, wave_number)

	# Fall back to all available if the specific type has no entries
	if primary_candidates.size() == 0:
		primary_candidates = _get_all_available(wave_number)

	# Primary fill (~70 % of the group's budget)
	var primary_spend: int = int(group_budget * _config.primary_pressure_ratio)
	if full_only:
		primary_spend = group_budget
	group_budget = _fill_budget(group.enemies, primary_candidates, primary_spend, group_budget)

	if full_only:
		group.enemies.shuffle()
		return group

	# Secondary fill (remaining budget with any unlocked type)
	var mixed_candidates: Array[EnemyData] = _get_all_available(wave_number)
	group_budget = _fill_budget(group.enemies, mixed_candidates, group_budget, group_budget)

	# Shuffle within the group so spawn order feels organic
	group.enemies.shuffle()
	return group

# ---------------------------------------------------------
# CATALOG FILTERS
# ---------------------------------------------------------

## Returns enemies of [param type] that are unlocked for [param wave_number].
func _get_available(type: EnemyData.Type, wave_number: int) -> Array[EnemyData]:
	return _enemy_catalog.filter(func(data: EnemyData) -> bool:
		return data.type == type and _is_unlocked(data, wave_number)
	)

## Returns every non-boss enemy unlocked for [param wave_number].
func _get_all_available(wave_number: int) -> Array[EnemyData]:
	return _enemy_catalog.filter(func(data: EnemyData) -> bool:
		return data.type != EnemyData.Type.BOSS and _is_unlocked(data, wave_number)
	)

## Whether a given enemy type is available at [param wave_number].
func _is_type_unlocked(type: EnemyData.Type, wave_number: int) -> bool:
	match type:
		EnemyData.Type.BOSS:
			return wave_number >= _config.boss_unlock_wave
		_:
			# SWARM, FAST, NORMAL and TANK are always available.
			# Per-wave availability is controlled by EnemyData.available_waves.
			return true

## Whether a specific enemy is available at [param wave_number].
## Checks both the type-level unlock (from WaveConfig) and the
## per-enemy ranges (from EnemyData.available_waves).
func _is_unlocked(data: EnemyData, wave_number: int) -> bool:
	return _is_type_unlocked(data.type, wave_number) and _is_wave_available_for_enemy(data, wave_number)

## True when [param wave_number] falls inside one of [param data]'s ranges.
## Empty ranges are treated as always available (legacy-compatible fallback).
func _is_wave_available_for_enemy(data: EnemyData, wave_number: int) -> bool:
	if data.available_waves.is_empty():
		return true

	for wave_range in data.available_waves:
		if wave_range == null:
			continue
		if _is_wave_in_range(wave_number, wave_range):
			return true

	return false

## Inclusive range check. A final wave <= 0 means "no upper bound".
func _is_wave_in_range(wave_number: int, wave_range: EnemyWaveRange) -> bool:
	if wave_number < wave_range.initial_wave:
		return false
	if wave_range.final_wave <= 0:
		return true
	return wave_number <= wave_range.final_wave

# ---------------------------------------------------------
# BUDGET FILLING
# ---------------------------------------------------------

## Picks random affordable enemies from [param candidates] until
## [param target_spend] or [param total_remaining] is exhausted.
## Returns the updated remaining budget.
func _fill_budget(
	result: Array[EnemyData],
	candidates: Array[EnemyData],
	target_spend: int,
	total_remaining: int
) -> int:
	var spent: int = 0

	while spent < target_spend and total_remaining > 0:
		var affordable: Array[EnemyData] = candidates.filter(
			func(e: EnemyData) -> bool: return e.weight <= total_remaining
		)
		if affordable.size() == 0:
			break

		var pick: EnemyData = affordable[randi() % affordable.size()]
		result.append(pick)
		spent += pick.weight
		total_remaining -= pick.weight

	return total_remaining
