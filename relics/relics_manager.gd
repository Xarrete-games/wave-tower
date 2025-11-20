#Reliocs Manager
extends Node

signal relics_change(relics: Array[Relic])

var relics: Dictionary[String, Relic] = {
}

func reset_relics() -> void:
	relics = {}
	relics_change.emit([] as Array[Relic])

func add_relic(relic: Relic) -> void:
	relic.apply_effect()		
	_add_relic(relic)

func _add_relic(relic: Relic) -> void:
	if relics.has(relic.get_id()):
		(relics[relic.get_id()] as Relic).amount += 1
	else:
		relics[relic.get_id()] = relic
	relics_change.emit(relics.values())
