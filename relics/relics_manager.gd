#Reliocs Manager
extends Node

signal relics_change(relics: Array[Relic])
signal relic_added(relic: Relic)

var relics: Dictionary[String, Relic] = {
}

func reset_relics() -> void:
	relics = {}
	relics_change.emit([] as Array[Relic])

func add_relic(relic: Relic, is_free = false) -> void:
	relic.apply_effect()
	# increase priece in 20%
	if not is_free:
		Score.gold -= relic.price
	relic.price = round(relic.price + (relic.price * 0.2))
	_add_relic(relic)

func _add_relic(relic: Relic) -> void:
	if relics.has(relic.id):
		(relics[relic.id] as Relic).amount += 1
	else:
		relics[relic.id] = relic
	relics_change.emit(relics.values())
	relic_added.emit(relic)
