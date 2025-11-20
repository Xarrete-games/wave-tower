class_name RelicUI extends Control

@onready var texture: TextureRect = $MarginContainer/texture
@onready var amount: Label = $MarginContainer/amount
@onready var relic_description: RelicDescriptionHover = $TopBarRelicDescription

func _ready() -> void:
	relic_description.visible = false

func get_texture():
	return texture

func get_amount():
	return amount

func _on_mouse_entered() -> void:
	relic_description.visible = true

func _on_mouse_exited() -> void:
	relic_description.visible = false
