class_name TowersManager extends RefCounted

signal tower_count_change(tower_type: Tower.Type, amount: int)
signal tower_card_amount_change(tower_configuration: TowerDataWithInstance, amount: int)
signal tower_placed(tower: Tower)
signal tower_hovered(tower: Tower)
signal tower_unhovered(tower: Tower)

const INITIAL_TOWERS_IDS = ["fire_tower", "frost_tower", "lightning_tower"]


var last_tower_ids: Dictionary[String, int] = {
	
}

var towers_ids: Array[String] = []
var towers: Array[Tower] = []
var all_tower_data: Array[TowerDataWithInstance] = []
var tower_cards_amount: Dictionary[String, int] = {}

func _init() -> void:
	ClickEvents.tower_remove_pressed.connect(tower_removed)
	ClickEvents.add_tower_card.connect(_on_tower_card_added)
	all_tower_data = DataLoader.get_all_tower_data()
	_init_inital_towers_data()

func get_random_towers(amount: int) -> Array[TowerDataWithInstance]:
	var available_towers = all_tower_data.duplicate()
	available_towers.shuffle()
	return available_towers.slice(0, amount)

func reset_towers() -> void:
	_update_tower_count(Tower.Type.FIRE)
	_update_tower_count(Tower.Type.LIGHTNING)
	_update_tower_count(Tower.Type.FROST)

func get_tower_configuration_by_id(id: String) -> TowerDataWithInstance:
	for tower_configuration in all_tower_data:
		if tower_configuration.data.id == id:
			return tower_configuration
	return null

# called from tower_placer to inform
func add_tower_placed(tower: Tower) -> void:
	Hooks.on_tower_placed(tower)
	towers.append(tower)
	_update_tower_count(tower.type)
	tower_cards_amount[tower.data.id] -= 1
	var tower_configuration = get_tower_configuration_by_id(tower.data.id)
	tower_card_amount_change.emit(tower_configuration, tower_cards_amount[tower.data.id])
	tower.id = _generate_tower_id(tower)
	tower_placed.emit(tower)

func tower_removed(tower: Tower) -> void:
	towers.erase(tower)
	_update_tower_count(tower.type)
	towers_ids.erase(tower.id)
	tower.queue_free()

func get_tower_count(tower_type: Tower.Type) -> int:
	var count = 0
	for tower in towers:
		if tower.type == tower_type:
			count += 1
	return count

func _update_tower_count(tower_type: Tower.Type) -> void:
	tower_count_change.emit(tower_type, get_tower_count(tower_type))

func _init_inital_towers_data() -> void:
	var inital_cards = []

	for tower_id in INITIAL_TOWERS_IDS:
		var tower_configuration = get_tower_configuration_by_id(tower_id)
		if tower_configuration != null:
			inital_cards.append(tower_configuration)

	for tower_configuration in inital_cards:
		_on_tower_card_added(tower_configuration)


func _on_tower_card_added(tower_data: TowerDataWithInstance) -> void:
	if tower_data.data.id in tower_cards_amount:
		tower_cards_amount[tower_data.data.id] += 1
	else:
		tower_cards_amount[tower_data.data.id] = 1
	tower_card_amount_change.emit(tower_data, tower_cards_amount[tower_data.data.id])
	
func _generate_tower_id(tower: Tower) -> String:
	var base_id = tower.type_id
	if not last_tower_ids.has(base_id):
		last_tower_ids[base_id] = 0

	var count = last_tower_ids[base_id] + 1
	var new_id = base_id + "_" + str(count)
	while new_id in towers_ids:
		count += 1
		new_id = base_id + "_" + str(count)
	towers_ids.append(new_id)
	last_tower_ids[base_id] = count
	return new_id
