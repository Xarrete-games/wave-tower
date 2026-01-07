class_name RelicUI extends Control

const semi_transparent_color: Color = Color(1, 1, 1, 0.5)
const opaque_color: Color = Color(1, 1, 1, 1)

@onready var relic_description: RelicDescriptionHover = $VBoxContainer/TopBarRelicDescription
@onready var texture: TextureRect = $VBoxContainer/MarginContainer/texture
@onready var amount: Label = $VBoxContainer/MarginContainer/amount

func _ready() -> void:
	relic_description.visible = false

func set_relic(relic: Relic) -> void:
	texture.texture = relic.texture

	if relic.disabled:
		texture.modulate = semi_transparent_color
	else:
		texture.modulate = opaque_color

	if relic.amount > 1:
		amount.text = str(relic.amount)
	relic_description.set_description(relic.description) 

func _on_mouse_entered() -> void:
	relic_description.visible = true

func _on_mouse_exited() -> void:
	relic_description.visible = false
