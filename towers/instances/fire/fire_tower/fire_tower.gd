class_name FireTower extends UpgradeableTower

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
	var debuff = RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.BURN) if apply_burn else null
	fire_ball.set_target(_current_target, attack, debuff)

func _on_extra_stats_change(tower_extra_stats: TowerExtraStats) -> void:
	apply_burn = tower_extra_stats.all_fire_apply_burn
