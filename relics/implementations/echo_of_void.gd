class_name EchoOfVoid extends Relic

func on_obtain() -> void:
	for tower in RunContext.towers_manager.towers:
		if tower is LightningChainTower:
			_add_buff(tower)

func on_tower_place(tower: Tower) -> void:
	_add_buff(tower)

func on_remove() -> void:
	for tower in RunContext.towers_manager.towers:
		if tower is LightningChainTower:
			_remove_buff(tower)

func _add_buff(tower: Tower) -> void:
	if tower is LightningChainTower:
		var lightning_chain_tower = tower as LightningChainTower
		lightning_chain_tower.current_bounces += 1

func _remove_buff(tower: Tower) -> void:
	if tower is LightningChainTower:
		var lightning_chain_tower = tower as LightningChainTower
		lightning_chain_tower.current_bounces -= 1

