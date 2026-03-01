class_name WaveSpawner extends Node
## Handles the physical spawning of enemies during a wave.
##
## Receives an Array[WaveComposer.WaveGroup] from the composer.
## Each group is spawned sequentially — enemies within a group are
## separated by spawn_interval, and a longer group_delay sits between
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
## [param spawn_interval] — seconds between enemies within a group.
## [param group_delay]    — seconds between the end of one group and the start of the next.
func start_wave(
	wave_number: int,
	groups: Array[WaveComposer.WaveGroup],
	spawn_interval: float,
	group_delay: float
) -> void:
	if _is_spawning:
		push_warning("[WaveSpawner] Already spawning a wave — ignoring request")
		return
	if world_map == null:
		push_warning("[WaveSpawner] No world_map assigned")
		return

	_is_spawning = true
	wave_started.emit(wave_number)

	for group_idx in range(groups.size()):
		var group: WaveComposer.WaveGroup = groups[group_idx]

		for enemy_idx in range(group.enemies.size()):
			_spawn_single(group.enemies[enemy_idx])
			# Delay between individual enemies (skip after last in group)
			if enemy_idx < group.enemies.size() - 1:
				await get_tree().create_timer(spawn_interval).timeout

		group_finished.emit(group_idx, group.pressure)

		# Delay between groups (skip after last group)
		if group_idx < groups.size() - 1:
			await get_tree().create_timer(group_delay).timeout

	_is_spawning = false
	wave_finished.emit(wave_number)

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

	# Spawn directly at the first waypoint to avoid awkward transition from portal
	enemy.global_position = waypoints[0]
	enemy.set_waypoints(waypoints)

	enemy_spawned.emit(enemy)

## Transfers EnemyData stats onto an Enemy instance.
func _apply_stats(enemy: Enemy, data: EnemyData) -> void:
	enemy.max_health = data.max_health
	enemy.base_speed = data.base_speed
	enemy.damage = data.damage
	enemy.base_gold_value = data.base_gold_value
