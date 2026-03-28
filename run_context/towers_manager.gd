class_name TowersManager extends RefCounted

signal tower_count_change(tower_type: Tower.Type, amount: int)
signal tower_card_amount_change(tower_configuration: TowerDataWithInstance, amount: int)
signal tower_placed(tower: Tower)
signal tower_hovered(tower: Tower)
signal tower_unhovered(tower: Tower)

const INITIAL_TOWERS_IDS = ["fire_tower", "frost_tower", "lightning_tower"]

var towers_placed: Dictionary[Tower.Type, int] = {
	Tower.Type.FIRE: 0,
	Tower.Type.FROST: 0,
	Tower.Type.LIGHTNING: 0
}

var last_tower_ids: Dictionary[String, int] = {
	
}

var towers_ids: Array[String] = []
var all_tower_data: Array[TowerDataWithInstance] = []
var tower_cards_amount: Dictionary[String, int] = {}

func _init() -> void:
	ClickEvents.tower_remove_pressed.connect(tower_removed)
	ClickEvents.add_tower_card.connect(_on_tower_card_added)
	RunContext.progress.current_level_changed.connect(_on_level_changed)
	all_tower_data = DataLoader.get_all_tower_data()
	_init_inital_towers_data()

func get_random_towers(amount: int) -> Array[TowerDataWithInstance]:
	var available_towers = all_tower_data.duplicate()
	available_towers.shuffle()
	return available_towers.slice(0, amount)

func reset_towers() -> void:
	_update_tower_count(Tower.Type.FIRE, 0)
	_update_tower_count(Tower.Type.LIGHTNING, 0)
	_update_tower_count(Tower.Type.FROST, 0)

func get_tower_configuration_by_id(id: String) -> TowerDataWithInstance:
	for tower_configuration in all_tower_data:
		if tower_configuration.data.id == id:
			return tower_configuration
	return null

# called from tower_placer to inform
func add_tower_placed(tower: Tower) -> void:
	Hooks.on_tower_placed(tower)
	_update_tower_count(tower.type, towers_placed[tower.type] + 1)
	tower_cards_amount[tower.data.id] -= 1
	var tower_configuration = get_tower_configuration_by_id(tower.data.id)
	tower_card_amount_change.emit(tower_configuration, tower_cards_amount[tower.data.id])
	tower.id = _generate_tower_id(tower)
	tower_placed.emit(tower)

func tower_removed(tower: Tower) -> void:
	var type = tower.type
	_update_tower_count(tower.type, towers_placed[tower.type] - 1)
	towers_ids.erase(tower.id)
	tower.queue_free()

func get_tower_count(tower_type: Tower.Type) -> int:
	return towers_placed[tower_type]

func _on_level_changed(_new_level: int) -> void:
	reset_towers()

func _update_tower_count(tower_type: Tower.Type, value: int) -> void:
	towers_placed[tower_type] = value
	tower_count_change.emit(tower_type, value)

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
