class_name FireTower extends UpgradeableTower

const FIRE_BALL_SCENE: PackedScene = preload("uid://c87kjybjulaxq")

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	if _current_target == null:
		return

	var fire_ball: FireBallProjectile = FIRE_BALL_SCENE.instantiate()
	add_child(fire_ball)
	fire_ball.global_position = projectile_spawn_pos.global_position
	var attack: Attack = _get_attack()
	fire_ball.set_target(_current_target, attack)

func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass