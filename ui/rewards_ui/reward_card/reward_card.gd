class_name Rewardard extends Control

signal card_pressed(relic: Relic)

var relic: Relic

@onready var description: RichTextLabel = $MarginContainer/MarginContainer/VBoxContainer/Description
@onready var title: Label = $MarginContainer/MarginContainer/VBoxContainer/Title
@onready var texture_button: TextureButton = $TextureButton

func set_relic(new_relic_value: Relic) -> void:
	relic = new_relic_value
	texture_button.texture_normal = relic.get_texture()
	title.text = relic.get_id()
	description.text = relic.get_description()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

func _on_texture_button_pressed() -> void:
	card_pressed.emit(relic)
