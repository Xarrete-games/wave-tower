class_name Rewardard extends Control

signal card_pressed(relic: Relic)

var relic: Relic

@onready var description: RichTextLabel = $MarginContainer/MarginContainer/VBoxContainer/Description
@onready var title: Label = $MarginContainer/MarginContainer/VBoxContainer/Title
@onready var relic_texture: TextureRect = $RelicTexture

func set_relic(new_relic_value: Relic) -> void:
	relic = new_relic_value
	relic_texture.texture = relic.get_texture()
	title.text = relic.get_id()
	description.text = relic.get_description()

func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and not event.is_pressed():
			card_pressed.emit(relic)
