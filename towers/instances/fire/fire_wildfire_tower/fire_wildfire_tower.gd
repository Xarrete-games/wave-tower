class_name WildFireTower extends Tower

@export var projectile_scene: PackedScene
@onready var projectile_spawn_pos: Marker2D = $ProjectileSpawnPos

func _fire() -> void:
	var projectile: SingleTargetProjectile = projectile_scene.instantiate()
	add_child(projectile)
	projectile.global_position = projectile_spawn_pos.global_position
	projectile.set_target(_current_target, _get_attack(), EnemyDebuff.create_burn(damage_source))
	
func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass
