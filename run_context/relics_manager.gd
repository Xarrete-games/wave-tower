class_name RelicsManager extends RefCounted

signal relics_change(relics: Array[Relic])
signal relic_added(relic: Relic)

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD

var relic_colors: Dictionary[Relic.Rarity, Color] = {
	Relic.Rarity.COMMON: COMMON_COLOR,
	Relic.Rarity.RARE: RARE_COLOR,
	Relic.Rarity.EPIC: EPIC_COLOR,
	Relic.Rarity.ALL: COMMON_COLOR,
}

var relics_count: Dictionary[String, int] = {} 

var relics: Dictionary[String, Relic] = {
}

func has_relic(relic_id: String) -> bool:
	return relics.has(relic_id) and not (relics[relic_id]).disabled

func get_all_relics() -> Array[Relic]:
	return relics.values()

func is_maxed(relic_id: String) -> bool:
	var count = relics_count.get(relic_id, 0)
	var relic = relics.get(relic_id, null)
	
	if relic == null:
		return false
	return count >= relic.max_stacks
	
func get_rarity_color(rarity: Relic.Rarity) -> Color:
	return relic_colors[rarity]

func reset_relics() -> void:
	relics = {}
	relics_change.emit([] as Array[Relic])

func add_relic(relic: Relic) -> void:
	relic.apply_effect()
	_add_relic(relic)

func remove_relic(relic_id: String) -> void:
	if relics.has(relic_id):
		relics[relic_id].remove_effect()
		relics.erase(relic_id)
		relics_count[relic_id] = relics_count.get(relic_id, 0) - 1
		relics_change.emit(relics.values())

func disable_relic(relic_id: String) -> void:
	if relics.has(relic_id):
		(relics[relic_id] as Relic).disabled = true
		relics_change.emit(relics.values())

func _add_relic(relic: Relic) -> void:
	if relics.has(relic.id):
		(relics[relic.id] as Relic).amount += 1
	else:
		relics[relic.id] = relic
	relics_count[relic.id] = relics_count.get(relic.id, 0) + 1
	relics_change.emit(relics.values())
	relic_added.emit(relic)
