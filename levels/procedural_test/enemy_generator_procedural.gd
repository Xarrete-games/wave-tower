class_name EnemyGeneratorProcedural extends Node

const ENEMY_SCENE: PackedScene = preload("uid://b4grxp1f6om7o")

@export var world_map: WorldMap = null


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("ui_accept"):
		await get_tree().create_timer(0.1).timeout
		spawn_enemy()


## Spawns an enemy at a random portal and starts it moving toward the target
func spawn_enemy() -> void:
	if world_map == null:
		push_warning("[EnemyGenerator] No world_map assigned")
		return
	
	var portal_entries: Array[Dictionary] = world_map.portal_entries
	if portal_entries.size() == 0:
		push_warning("[EnemyGenerator] No spawn points available")
		return
	
	# Choose random spawn
	var spawn_entry: Dictionary = portal_entries[randi() % portal_entries.size()]
	
	# Generate waypoints for that route
	var waypoints: Array[Vector2] = world_map.get_waypoints_for_spawn(spawn_entry)
	
	if waypoints.size() == 0:
		push_warning("[EnemyGenerator] Could not generate waypoints")
		return
	
	# Create enemy instance
	var enemy: EnemyProcedural = ENEMY_SCENE.instantiate()
	add_child(enemy)
	
	# Position at spawn point
	enemy.global_position = spawn_entry["pos"]
	
	# Assign waypoints and start moving
	enemy.set_waypoints(waypoints)
