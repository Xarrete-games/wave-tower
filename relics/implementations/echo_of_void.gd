class_name EchoOfVoid extends TowerBuffRelic

func _add_buff(tower: Tower) -> void:
	var lightning_chain_tower = tower as LightningChainTower
	lightning_chain_tower.current_bounces += 1

func _remove_buff(tower: Tower) -> void:
	var lightning_chain_tower = tower as LightningChainTower
	lightning_chain_tower.current_bounces -= 1

func _is_valid_tower(tower: Tower) -> bool:
	return tower is LightningChainTower