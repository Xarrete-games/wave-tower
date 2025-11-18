#Reliocs Manager
extends Node

signal relics_change(relics_array: Array[Relic])

var relics: Dictionary = {
}

func reset_relics() -> void:
	relics = {}
	relics_change.emit([])

func add_relic(towers_buffs: Dictionary[Tower.TowerType, TowerBuff], relic: Relic) -> void:
	match (relic.type):
		RedRelic.RelicType.RED:
			relic.apply_effect(towers_buffs[Tower.TowerType.RED])
		RedRelic.RelicType.GREEN:
			relic.apply_effect(towers_buffs[Tower.TowerType.GREEN])
		RedRelic.RelicType.BLUE:
			relic.apply_effect(towers_buffs[Tower.TowerType.BLUE])
		RedRelic.RelicType.COMMON:
			_apply_to_all(towers_buffs, relic)
			
	_add_relic(relic)

func _apply_to_all(towers_buffs: Dictionary[Tower.TowerType, TowerBuff], relic: Relic) -> void:
	for tower_type in Tower.TowerType.values():
		var tower_buffs = towers_buffs[tower_type]
		relic.apply_effect(tower_buffs)

func _add_relic(relic: Relic) -> void:
	if relics.has(relic.get_id()):
		(relics[relic.get_id()] as Relic).amount += 1
	else:
		relics[relic.get_id()] = relic
	relics_change.emit(relics.values())
