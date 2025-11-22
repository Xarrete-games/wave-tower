class_name RelicUI extends Control

@onready var texture: TextureRect = $MarginContainer/texture
@onready var amount: Label = $MarginContainer/amount
@onready var relic_description: RelicDescriptionHover = $TopBarRelicDescription

func _ready() -> void:
	relic_description.visible = false

func set_relic(relic: Relic) -> void:
	texture.texture = relic.texture
	if relic.amount > 1:
		amount.text = str(relic.amount)
	relic_description.set_description(relic.description) 

func _on_mouse_entered() -> void:
	var mouse_pos = get_viewport().get_mouse_position()
	relic_description.offset = mouse_pos
	relic_description.visible = true

func _on_mouse_exited() -> void:
	relic_description.visible = false
