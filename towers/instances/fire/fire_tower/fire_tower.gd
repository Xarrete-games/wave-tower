class_name FireTower extends Tower

@export var fire_ball_scene: PackedScene

var apply_burn: bool = false

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	if _current_target == null:
		return

	var fire_ball: SingleTargetProjectile = fire_ball_scene.instantiate()
	add_child(fire_ball)
	fire_ball.global_position = projectile_spawn_pos.global_position
	var attack: Attack = _get_attack()
	var debuff = EnemyDebuff.create_burn(damage_source) if apply_burn else null
	fire_ball.set_target(_current_target, attack, debuff)
