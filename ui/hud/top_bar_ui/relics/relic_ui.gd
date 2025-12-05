class_name RelicUI extends Control

@onready var relic_description: RelicDescriptionHover = $VBoxContainer/TopBarRelicDescription
@onready var texture: TextureRect = $VBoxContainer/MarginContainer/texture
@onready var amount: Label = $VBoxContainer/MarginContainer/amount

func _ready() -> void:
	relic_description.visible = false

func set_relic(relic: Relic) -> void:
	texture.texture = relic.texture
	if relic.amount > 1:
		amount.text = str(relic.amount)
	relic_description.set_description(relic.description) 

func _on_mouse_entered() -> void:
	relic_description.visible = true

func _on_mouse_exited() -> void:
	relic_description.visible = false
