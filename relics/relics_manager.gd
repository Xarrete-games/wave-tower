#Reliocs Manager
extends Node

signal relics_change(relics: Dictionary)

var relics: Dictionary = {
}

func reset_relics() -> void:
	relics = {}
	relics_change.emit(relics)


func add_relic(all_towers_stats: Dictionary, relic: RedRelic) -> void:
	_apply_to_all(all_towers_stats, relic)

func add_red_relic(all_towers_stats: Dictionary) -> void:
	var relic = RedRelic.new()
	_apply_to_all(all_towers_stats, relic)

func add_green_relic(all_towers_stats: Dictionary) -> void:
	var relic = GreenRelic.new()
	_apply_to_all(all_towers_stats, relic)

func add_blue_relic(all_towers_stats: Dictionary) -> void:
	var relic = BlueRelic.new()
	_apply_to_all(all_towers_stats, relic)

func _apply_to_all(all_towers_stats: Dictionary, relic: Relic) -> void:
	for tower_type in Tower.TowerType.values():
		var tower_stats = all_towers_stats[tower_type]
		relic.apply_effect(tower_stats)
	_add_relic(relic)

func _add_relic(relic: Relic) -> void:
	if relics.has(relic.get_id()):
		(relics[relic.get_id()] as Relic).amount += 1
	else:
		relics[relic.get_id()] = relic
	relics_change.emit(relics)
