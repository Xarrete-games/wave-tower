class_name HealthBar extends Control

const DEBUFF_SLOT = preload("uid://beixhmpysku3t")
const BURN_ICON = preload("uid://deriu0icenegb")
const FROST_ICON = preload("uid://jgpoavtm56v2")



# Rango de Salud
const MIN_HEALTH: float = 40.0
const MAX_HEALTH: float = 5000.0

# Rango de Escala Visual
const MIN_SCALE: float = 1.0
const MAX_SCALE: float = 4.0

@export var debuffs_conatiner: Control
@export var texture_progress_bar: TextureProgressBar 
var debuffs_slots: Dictionary[EnemyDebuff.Type, DebuffSlot] = {
	EnemyDebuff.Type.BURN: null,
	EnemyDebuff.Type.FROST: null
}

var textures: Dictionary[EnemyDebuff.Type, Texture2D] = {
	EnemyDebuff.Type.BURN: BURN_ICON,
	EnemyDebuff.Type.FROST: FROST_ICON
} 


func set_max_health(value: float) -> void:
	var clamped_value = clamp(value, MIN_HEALTH, MAX_HEALTH)
	var new_scale = remap(clamped_value, MIN_HEALTH, MAX_HEALTH, MIN_SCALE, MAX_SCALE)
	self.scale = Vector2(new_scale, 1)
	
	texture_progress_bar.max_value = value
	
func update_health(new_value: float) -> void:
	texture_progress_bar.value = new_value
	

func set_debuffs(debuffs: Dictionary[EnemyDebuff.Type, int]) -> void:
	for type in EnemyDebuff.Type.values():
		var value = debuffs[type]
		if value == 0:
			_remove_debuff(type)
		else:
			_update_value(type, value)

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
		
	

	
