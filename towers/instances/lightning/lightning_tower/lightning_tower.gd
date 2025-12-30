class_name LightningTower extends UpgradeableTower

const ELECTRIC_BALL_SCENE: PackedScene = preload("uid://boewdbtx7u2f7")

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	if _current_target == null:
		return

	var electric_ball: SingleTargetProjectile = ELECTRIC_BALL_SCENE.instantiate()
	add_child(electric_ball)
	electric_ball.global_position = projectile_spawn_pos.global_position
	var attack: Attack = _get_attack()
	electric_ball.set_target(_current_target, attack)

func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass