#RelicCalog
extends Node

const RELICS_DATA_PATH = "res://items/relics/data/"

var relics_by_id: Dictionary[String, ItemData] = {}
var all_relics: Array[ItemData] = []

func _ready() -> void:
	_load_relic_data()

func get_by_id(id: String) -> ItemData:
	return relics_by_id.get(id)

func get_all() -> Array[ItemData]:
	return all_relics

func get_by_rarity(rarity: Relic.Rarity) -> Array[ItemData]:
	var result: Array[ItemData] = []
	for relic in all_relics:
		if relic.rarity == rarity:
			result.append(relic)
	return result

func _load_relic_data() -> void:
	relics_by_id.clear()
	all_relics.clear()

	var dir: DirAccess = DirAccess.open(RELICS_DATA_PATH)
	if dir == null:
		push_error("[RelicCatalog] Directory not found")
		return

	dir.list_dir_begin()
	var file_name: String = dir.get_next()

	while file_name != "":
		if file_name.ends_with(".tres"):
			var data: ItemData = load(RELICS_DATA_PATH + file_name)
			if data:
				all_relics.append(data)
				relics_by_id[data.id] = data
		file_name = dir.get_next()

	dir.list_dir_end()

	print("[RelicCatalog] Loaded ", all_relics.size(), " relics")