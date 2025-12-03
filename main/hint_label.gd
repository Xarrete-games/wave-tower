class_name HintLabel extends Label

const INITIAL_HINT = "[WASD] Move Camera"

var is_first_value: bool = true

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	text = INITIAL_HINT
	TowerPlacementManager.tower_placing.connect(_on_tower_placing)
	await get_tree().create_timer(10).timeout
	if text == INITIAL_HINT:
		text = ""

func _on_tower_placing(value: bool) -> void:
	if is_first_value:
		is_first_value = false
		return
	
	if not value:
		text = ""
	else:
		# Indica claramente la acción y la tecla
		text = "[ESC] Cancel"
	
