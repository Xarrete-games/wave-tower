class_name BlueTower extends Tower

const BLUE_PROJECTIL = preload("uid://csif0nju31dcs")

var double_shot_chance = 0

@onready var projectil_spawn_point: Marker2D = $ProjectilSpawnPoint

	
func _on_extra_stats_change(tower_extra_stats: TowerExtraStats) -> void:
	double_shot_chance = tower_extra_stats.double_shot_chance

func _fire() -> void:
	cristal_light.play()
	var projectil: BlueProjectil = BLUE_PROJECTIL.instantiate()
	
	var is_double_hit = _is_doble_hit()
	
	var attack = _get_attack()
	projectil.set_stats(attack, stats.attack_range, RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.FROST))
	call_deferred("_add_projectil", projectil)
	
	if is_double_hit:
		await get_tree().create_timer(0.5).timeout
		cristal_light.play()
		projectil = BLUE_PROJECTIL.instantiate()
		attack = _get_attack()
		projectil.set_stats(attack, stats.attack_range, RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.FROST))
		call_deferred("_add_projectil", projectil)

func _add_projectil(projectil: BlueProjectil) -> void:
	add_child(projectil)
	projectil.position = projectil_spawn_point.position

func _is_doble_hit() -> bool:
	var random = randf()
	return (double_shot_chance / 100) >= random
