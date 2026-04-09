class_name HealthBar extends Control

const DEBUFF_SLOT = preload("uid://beixhmpysku3t")


# Rango de Salud
const MIN_HEALTH: float = 40.0
const MAX_HEALTH: float = 5000.0

# Rango de Escala Visual
const MIN_X_SIZE: float = 40.0
const MAX_X_SIZE: float = 160.0

@export var debuffs_conatiner: Control
@export var texture_progress_bar: TextureProgressBar 

var debuffs_slots: Dictionary[int, DebuffSlot] = {}
var debuffs_count: Dictionary[int, int] = {}
var debuff_data_by_type: Dictionary[int, Variant] = {}

func set_max_health(value: float) -> void:
	var clamped_value = clamp(value, MIN_HEALTH, MAX_HEALTH)
	var new_x_size = remap(
		clamped_value,
		MIN_HEALTH,
		MAX_HEALTH,
		MIN_X_SIZE,
		MAX_X_SIZE
	)

	texture_progress_bar.custom_minimum_size.x = new_x_size
	texture_progress_bar.max_value = value
	custom_minimum_size.x = new_x_size
	
func update_health(new_value: float) -> void:
	texture_progress_bar.value = new_value
	

func set_debuffs(debuffs: Array[EnemyDebuffInstance]) -> void:
	_reset_debuffs()
	for debuff_instance: EnemyDebuffInstance in debuffs:
		var debuff_type: int = debuff_instance.debuff.type
		debuffs_count[debuff_type] = debuffs_count.get(debuff_type, 0) + 1
		debuff_data_by_type[debuff_type] = debuff_instance.debuff.data

	for type in debuffs_slots.keys().duplicate():
		if not debuffs_count.has(type):
			_remove_debuff(type)

	for type in debuffs_count.keys():
		_update_debuff_value(type, debuffs_count[type], debuff_data_by_type[type])


func _reset_debuffs() -> void:
	debuffs_count = {}
	debuff_data_by_type = {}

func _update_debuff_value(type: int, value: int, debuff_data) -> void:
	var slot = debuffs_slots.get(type, null)
	if slot == null:
		_create_debuff_slot_type(type, debuff_data)
	debuffs_slots[type].amount = value

func _create_debuff_slot_type(type: int, debuff_data) -> void:
	var slot = DEBUFF_SLOT.instantiate()
	debuffs_conatiner.add_child(slot)
	slot.texture = debuff_data.icon
	debuffs_slots[type] = slot


func _remove_debuff(type: int) -> void:
	var slot = debuffs_slots.get(type, null)
	if slot == null:
		return
	
	slot.queue_free()
	debuffs_slots[type] = null
