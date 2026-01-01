class_name LightningChainTower extends Tower

@export var lightning_chain_projectile_scene: PackedScene

@onready var projectile_spawn_point: Marker2D = $ProjectileSpawnPos


func _fire() -> void:
	var projectile = lightning_chain_projectile_scene.instantiate() as LightningChainProjectile
	add_child(projectile)
	projectile.global_position = projectile_spawn_point.global_position
	projectile.set_target(_current_target, _get_attack())
	

func _on_extra_stats_change(_tower_extra_stats: TowerExtraStats) -> void:
	pass
