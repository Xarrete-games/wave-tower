class_name FrostTower extends UpgradeableTower

const FROST_BALL: PackedScene = preload("uid://cibktj8x8j1t8")

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	if _current_target == null:
		return

	var frost_ball: SingleTargetProjectile = FROST_BALL.instantiate()
	add_child(frost_ball)
	frost_ball.global_position = projectile_spawn_pos.global_position
	var attack: Attack = _get_attack()
	var debuff = RunContext.enemy_debuff_manager.get_debuff(EnemyDebuff.Type.FROST, damage_source)
	frost_ball.set_target(_current_target, attack, debuff)

func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass
