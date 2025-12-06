#Reliocs Manager
extends Node

signal relics_change(relics: Array[Relic])
signal relic_added(relic: Relic)

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD

var relic_colors: Dictionary[Relic.Rarity, Color] = {
	Relic.Rarity.COMMON: COMMON_COLOR,
	Relic.Rarity.RARE: RARE_COLOR,
	Relic.Rarity.EPIC: EPIC_COLOR,
}

var relics: Dictionary[String, Relic] = {
}

func get_rarity_color(rarity: Relic.Rarity) -> Color:
	return relic_colors[rarity]

func reset_relics() -> void:
	relics = {}
	relics_change.emit([] as Array[Relic])

func add_relic(relic: Relic, is_free: bool = false) -> void:
	relic.apply_effect()
	# increase priece in 20%
	if not is_free:
		Score.gold -= relic.price
	if relic.price_increased:
		relic.price = round(relic.price + (relic.price * 0.3))
	_add_relic(relic)

func _add_relic(relic: Relic) -> void:
	if relics.has(relic.id):
		(relics[relic.id] as Relic).amount += 1
	else:
		relics[relic.id] = relic
	relics_change.emit(relics.values())
	relic_added.emit(relic)
