class_name ElectricWaveTower extends Tower

const GREEN_PROJECTILE = preload("uid://ck6mf6m73ergh")

var num_waves = 3
var local_num_waves = 3

@onready var projectil_spawn_position: Marker2D = $ProjectilSpawnPosition
@onready var attack_player: AudioStreamPlayer2D = $AttackPlayer

func _on_extra_stats_change(tower_extra_stats: TowerExtraStats) -> void:
	num_waves = local_num_waves + tower_extra_stats.extra_waves
	
func _fire() -> void:
	#attack_player.play()
	for i in range(num_waves):
		if not _current_target:
			await get_tree().create_timer(0.1).timeout
			continue
		var projectil: GreenProjectile = GREEN_PROJECTILE.instantiate()
		call_deferred("_fire_projectil", projectil)
		await get_tree().create_timer(0.1).timeout

func _fire_projectil(projectil: GreenProjectile) -> void:
	if not _current_target:
		return
	
	cristal_light.play()
	
	const START_OFFSET_PERCENTAGE: float = 0.4
	
	var start_pos: Vector2 = projectil_spawn_position.global_position
	var target_pos: Vector2 = _current_target.global_position
	
	# position beetween projectil spawn and enemy
	var accelerated_start_pos: Vector2 = start_pos.lerp(target_pos, START_OFFSET_PERCENTAGE)
	
	add_child(projectil)
	projectil.global_position = accelerated_start_pos
	
	# configure attack
	var dir: Vector2 = (target_pos - projectil.global_position).normalized()
	var attack = _get_attack()
	
	projectil.set_direction(dir, attack)
