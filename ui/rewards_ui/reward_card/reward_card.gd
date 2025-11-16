class_name Rewardard extends Control

signal card_pressed(relic: Relic)

var relic: Relic

func set_relic(new_relic_value: Relic) -> void:
	relic = new_relic_value
	pass

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

func _on_texture_button_pressed() -> void:
	card_pressed.emit(relic)
