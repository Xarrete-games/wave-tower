class_name LightningTower extends UpgradeableTower

@export var electric_ball_scene: PackedScene

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	if _current_target == null:
		return

	var electric_ball: SingleTargetProjectile = electric_ball_scene.instantiate()
	add_child(electric_ball)
	electric_ball.global_position = projectile_spawn_pos.global_position
	var attack: Attack = _get_attack()
	electric_ball.set_target(_current_target, attack)
