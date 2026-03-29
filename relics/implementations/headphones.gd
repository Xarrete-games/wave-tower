class_name HeadPhones extends Relic

const EXTRA_CHANCE = 0.2

func on_obtain() -> void:
	for tower in RunContext.towers_manager.towers:
		_add_buff(tower)

func on_tower_place(tower: Tower) -> void:
	_add_buff(tower)

func on_remove() -> void:
	for tower in RunContext.towers_manager.towers:
		_remove_buff(tower)

func _add_buff(tower: Tower) -> void:
	if tower is FrostNovaTower:
		var frost_nova_tower = tower as FrostNovaTower
		frost_nova_tower.double_shot_chance += EXTRA_CHANCE

func _remove_buff(tower: Tower) -> void:
	if tower is FrostNovaTower:
		var frost_nova_tower = tower as FrostNovaTower
		frost_nova_tower.double_shot_chance -= EXTRA_CHANCE