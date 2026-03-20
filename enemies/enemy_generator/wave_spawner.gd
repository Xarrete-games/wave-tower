class_name WaveSpawner extends Node
## Handles the physical spawning of enemies during a wave.
##
## Receives an Array[WaveComposer.WaveGroup] from the composer.
## Each group is spawned sequentially — enemies within a group are
## separated by a pressure-specific random interval range, and a longer
## group_delay sits between
## groups so the player can feel the shift in pressure.
##
## Enemy scenes come from each EnemyData.scene (loaded via DataLoader),
## so different enemy types use their own visuals and animations.

# ---------------------------------------------------------
# SIGNALS
# ---------------------------------------------------------

## Emitted when the spawner begins dispatching the first group.
signal wave_started(wave_number: int)

## Emitted every time a single enemy is placed on the map.
signal enemy_spawned(enemy: Enemy)

## Emitted when a group finishes spawning (useful for UI / debug).
signal group_finished(group_index: int, pressure: WaveComposer.PressureType)

## Emitted after the last enemy of the last group has been spawned.
signal wave_finished(wave_number: int)

# ---------------------------------------------------------
# CONFIGURATION
# ---------------------------------------------------------

## Reference to the WorldMap that provides portal entries and waypoints.
@export var world_map: WorldMap = null

## Fallback scene used when an EnemyData has no scene assigned.
## Point this to a default Enemy scene in the inspector.
@export var fallback_enemy_scene: PackedScene = null

# ---------------------------------------------------------
# INTERNAL STATE
# ---------------------------------------------------------

var enemies_container: Node2D
var _is_spawning: bool = false

# ---------------------------------------------------------
# PUBLIC API
# ---------------------------------------------------------

## Spawns all groups sequentially.
## [param config] — wave pacing config (spawn ranges + group delay).
func start_wave(
	wave_number: int,
	groups: Array[WaveComposer.WaveGroup],
	config: WaveConfig
) -> void:
	if _is_spawning:
		push_warning("[WaveSpawner] Already spawning a wave — ignoring request")
		return
	if world_map == null:
		push_warning("[WaveSpawner] No world_map assigned")
		return
	if config == null:
		push_warning("[WaveSpawner] No WaveConfig provided")
		return

	_is_spawning = true
	wave_started.emit(wave_number)

	for group_idx in range(groups.size()):
		var group: WaveComposer.WaveGroup = groups[group_idx]

		for enemy_idx in range(group.enemies.size()):
			_spawn_single(group.enemies[enemy_idx])
			# Delay between individual enemies (skip after last in group)
			if enemy_idx < group.enemies.size() - 1:
				var spawn_interval: float = _pick_spawn_interval(group.pressure, wave_number, config)
				await get_tree().create_timer(spawn_interval).timeout

		group_finished.emit(group_idx, group.pressure)

		# Delay between groups (skip after last group)
		if group_idx < groups.size() - 1:
			await get_tree().create_timer(config.group_delay).timeout

	_is_spawning = false
	wave_finished.emit(wave_number)

func _pick_spawn_interval(
	pressure: WaveComposer.PressureType,
	wave_number: int,
	config: WaveConfig
) -> float:
	var interval_range: Vector2 = _get_spawn_interval_range(pressure, config)
	var decayed_range: Vector2 = _get_decayed_spawn_interval_range(interval_range, wave_number, config)
	var min_interval: float = maxf(decayed_range.x, 0.01)
	var max_interval: float = maxf(decayed_range.y, min_interval)
	max_interval = maxf(max_interval, min_interval)

	if is_equal_approx(min_interval, max_interval):
		return min_interval

	return randf_range(min_interval, max_interval)

func _get_spawn_interval_range(
	pressure: WaveComposer.PressureType,
	config: WaveConfig
) -> Vector2:
	match pressure:
		WaveComposer.PressureType.SWARM:
			return Vector2(config.spawn_interval_swarm_min, config.spawn_interval_swarm_max)
		WaveComposer.PressureType.SPEED:
			return Vector2(config.spawn_interval_speed_min, config.spawn_interval_speed_max)
		WaveComposer.PressureType.TANK:
			return Vector2(config.spawn_interval_tank_min, config.spawn_interval_tank_max)
		_:
			# Fallback pacing uses NORMAL interval values.
			return Vector2(config.spawn_interval_normal_min, config.spawn_interval_normal_max)

func _get_decayed_spawn_interval_range(
	base_range: Vector2,
	wave_number: int,
	config: WaveConfig
) -> Vector2:
	var every_waves: int = maxi(config.spawn_interval_max_decay_every_waves, 1)
	@warning_ignore("integer_division")
	var decay_steps: int = maxi((wave_number - 1) / every_waves, 0)
	var decay_amount: float = float(decay_steps) * config.spawn_interval_max_decay_amount

	var decayed_min: float = maxf(base_range.x - decay_amount, 0.01)
	var decayed_max: float = maxf(base_range.y - decay_amount, 0.01)

	# Keep a valid range even after many decay steps.
	if decayed_max < decayed_min:
		decayed_max = decayed_min

	return Vector2(decayed_min, decayed_max)

# ---------------------------------------------------------
# INTERNAL HELPERS
# ---------------------------------------------------------

## Creates a single enemy from [param data], applies its stats,
## places it at a random portal, and sends it along waypoints.
func _spawn_single(data: EnemyData) -> void:
	var portal_entries: Array[Dictionary] = world_map.portal_entries
	if portal_entries.size() == 0:
		push_warning("[WaveSpawner] No spawn points available")
		return

	# Pick a random portal
	var spawn_entry: Dictionary = portal_entries[randi() % portal_entries.size()]
	var waypoints: Array[Vector2] = world_map.get_waypoints_for_spawn(spawn_entry)
	if waypoints.size() == 0:
		push_warning("[WaveSpawner] No waypoints for spawn entry")
		return

	# Use the scene defined in the EnemyData; fall back to generic if missing
	var scene: PackedScene = data.scene if data.scene != null else fallback_enemy_scene
	if scene == null:
		push_error("[WaveSpawner] No scene for enemy '%s' and no fallback set" % data.name)
		return

	var enemy: Enemy = scene.instantiate() as Enemy
	if enemy == null:
		push_error("[WaveSpawner] Scene for '%s' did not produce an Enemy" % data.name)
		return

	_apply_stats(enemy, data)
	enemy.add_to_group("enemy")
	enemy.enabled = false
	enemies_container.add_child(enemy)
	enemy.disable() # disable until positioned to avoid unwanted behavior (e.g. flying in from origin)

	# Spawn directly at the first waypoint to avoid awkward transition from portal
	enemy.global_position = waypoints[0]
	enemy.enable()
	enemy.set_waypoints(waypoints)

	enemy_spawned.emit(enemy)

## Transfers EnemyData stats onto an Enemy instance.
func _apply_stats(enemy: Enemy, data: EnemyData) -> void:
	enemy.max_health = data.max_health
	enemy.base_speed = data.base_speed
	enemy.damage = data.damage
	enemy.base_gold_value = data.base_gold_value
