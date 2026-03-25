class_name FrostSpearTower extends Tower

@export var frost_spear_projectile_scene: PackedScene
@export var debuff_stacks: int = 5

@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	if _current_target == null:
		return

	var frost_spear: SingleTargetProjectile = frost_spear_projectile_scene.instantiate()
	add_child(frost_spear)
	frost_spear.global_position = projectile_spawn_pos.global_position
	var attack: Attack = _get_attack()

	var enemy_frost_stacks = _current_target.get_debuff_stacks(EnemyDebuff.Type.FROST)
	var damage_multiplier: float = 1.0 + enemy_frost_stacks * 0.10
	attack.damage *= damage_multiplier

	var debuff = EnemyDebuff.create_frost(damage_source)
	frost_spear.set_target(_current_target, attack, debuff, debuff_stacks)

func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass
