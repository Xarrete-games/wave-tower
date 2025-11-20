class_name TopBarRelic extends Control

@onready var texture: TextureRect = $MarginContainer/texture
@onready var amount: Label = $MarginContainer/amount
@onready var top_bar_relic_description: TopBarRelicDescription = $TopBarRelicDescription

func _ready() -> void:
	top_bar_relic_description.visible = false

func get_texture():
	return texture

func get_amount():
	return amount

func _on_mouse_entered() -> void:
	top_bar_relic_description.visible = true

func _on_mouse_exited() -> void:
	top_bar_relic_description.visible = false
