class_name HealthBar extends Control

const DEBUFF_SLOT = preload("uid://beixhmpysku3t")

@export var burn_icon: Texture2D
@export var frost_icon: Texture2D


# Rango de Salud
const MIN_HEALTH: float = 40.0
const MAX_HEALTH: float = 5000.0

# Rango de Escala Visual
const MIN_X_SIZE: float = 40.0
const MAX_X_SIZE: float = 160.0
var base_x_position: float

@export var debuffs_conatiner: Control
@export var texture_progress_bar: TextureProgressBar 

var debuffs_slots: Dictionary[EnemyDebuff.Type, DebuffSlot] = {
	EnemyDebuff.Type.BURN: null,
	EnemyDebuff.Type.FROST: null
}

var textures: Dictionary[EnemyDebuff.Type, Texture2D] = {
	EnemyDebuff.Type.BURN: burn_icon,
	EnemyDebuff.Type.FROST: frost_icon
}

var debuffs_count: Dictionary[EnemyDebuff.Type, int] = {
	EnemyDebuff.Type.BURN: 0,
	EnemyDebuff.Type.FROST: 0
}

func _ready() -> void:
	textures = {
		EnemyDebuff.Type.BURN: burn_icon,
		EnemyDebuff.Type.FROST: frost_icon
	}
	base_x_position = position.x

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

	# Compensar solo el crecimiento desde el tamaño mínimo
	var extra_width: float = new_x_size - MIN_X_SIZE
	position.x = base_x_position - extra_width * 0.5
	
func update_health(new_value: float) -> void:
	texture_progress_bar.value = new_value
	

func set_debuffs(debuffs: Array[EnemyDebuffInstance]) -> void:
	_reset_debuffs()
	for debuff_instance: EnemyDebuffInstance in debuffs:
		debuffs_count[debuff_instance.debuff.type] += 1

	for type in debuffs_count.keys():
		var value = debuffs_count[type]
		if value == 0:
			_remove_debuff(type)
		else:
			_update_value(type, value)


func _reset_debuffs() -> void:
	debuffs_count = {
		EnemyDebuff.Type.BURN: 0,
		EnemyDebuff.Type.FROST: 0
	}

func _update_value(type: EnemyDebuff.Type, value: int) -> void:
	var slot = debuffs_slots[type]
	if slot == null:
		_create_slot_type(type)
	debuffs_slots[type].amount = value

func _create_slot_type(type: EnemyDebuff.Type) -> void:
	var slot = DEBUFF_SLOT.instantiate()
	debuffs_conatiner.add_child(slot)
	slot.texture = textures[type]
	debuffs_slots[type] = slot


func _remove_debuff(type: EnemyDebuff.Type) -> void:
	var slot = debuffs_slots[type]
	if slot == null:
		return
	
	slot.queue_free()
	debuffs_slots[type] = null
		
	

	
