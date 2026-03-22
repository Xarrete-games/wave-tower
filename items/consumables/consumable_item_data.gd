class_name ConsumableData extends ItemData

@export_group("Consumable")
@export var consumable_type: Consumable.Type
@export var targeting_type: ConsumableTargeteable.TargetType
@export var cursor_icon: Texture2D
@export var cursor_icon_used: Texture2D
@export var use_sound: AudioStream

@export_group("Script")
@export var runtime_script: Script

func create_item() -> Consumable:
	return runtime_script.new(self)
