class_name HeadPhones extends TowerBuffRelic

const EXTRA_CHANCE = 0.2

func _add_buff(tower: Tower) -> void:
	var frost_nova_tower = tower as FrostNovaTower
	frost_nova_tower.double_shot_chance += EXTRA_CHANCE

func _remove_buff(tower: Tower) -> void:
	var frost_nova_tower = tower as FrostNovaTower
	frost_nova_tower.double_shot_chance -= EXTRA_CHANCE

func _is_valid_tower(tower: Tower) -> bool:
	return tower is FrostNovaTower