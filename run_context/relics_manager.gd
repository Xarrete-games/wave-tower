class_name RelicsManager extends RefCounted

signal relic_changed(relic)
signal relic_added(relic)
signal relic_removed(relic_id: String)

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD

var relic_colors: Dictionary[int, Color] = {
	0: COMMON_COLOR,
	1: RARE_COLOR,
	2: EPIC_COLOR,
}

var relics_count: Dictionary[String, int] = {} 

var relics: Dictionary[String, Variant] = {
}

func has_relic(relic_id: String) -> bool:
	return relics.has(relic_id) and not (relics[relic_id]).disabled

func get_all_relics() -> Array:
	return relics.values()
	
func get_rarity_color(rarity: int) -> Color:
	return relic_colors[rarity]

func add_relic(relic) -> void:
	if relic == null:
		push_error("[RelicsManager] Attempted to add null relic instance")
		return

	if relics.has(relic.data.id):
		push_error("Relic with ID '%s' already exists. Cannot add duplicate relics." % relic.data.id)
		return

	AudioManager.play_relic_obtain()
	relic.on_obtain()
	_add_relic(relic)

func remove_relic(relic_id: String) -> void:
	if relics.has(relic_id):
		relics[relic_id].on_remove()
		relics[relic_id].changed.disconnect(emit_relic_changed)
		relics.erase(relic_id)
		relics_count[relic_id] = relics_count.get(relic_id, 0) - 1
		relic_removed.emit(relic_id)
		
func _add_relic(relic) -> void:
	relics[relic.data.id] = relic
	relics_count[relic.data.id] = relics_count.get(relic.data.id, 0) + 1
	
	relic_added.emit(relic)
	relic.changed.connect(emit_relic_changed)
	
func emit_relic_changed(relic) -> void:
	relic_changed.emit(relic)

func _get_relic(id: String) -> Variant:
	return relics[id]
