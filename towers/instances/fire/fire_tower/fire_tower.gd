class_name FireTower extends UpgradeableTower

@export var fire_ball_scene: PackedScene

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	if _current_target == null:
		return

	var fire_ball: SingleTargetProjectile = fire_ball_scene.instantiate()
	add_child(fire_ball)
	fire_ball.global_position = projectile_spawn_pos.global_position
	var attack: Attack = _get_attack()
	fire_ball.set_target(_current_target, attack)

func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass
