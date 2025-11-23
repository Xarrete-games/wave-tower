class_name Rewardard extends Control

signal card_pressed(relic: Relic)

var relic: Relic

@onready var description: RichTextLabel = $VBoxContainer/Description
@onready var title: Label = $Title
@onready var amount_gold_label: Label = $VBoxContainer/Gold/AmountGoldLabel

@onready var relic_texture: TextureRect = $RelicTexture

func set_relic(new_relic_value: Relic) -> void:
	relic = new_relic_value
	relic_texture.texture = relic.texture
	title.text = relic.id
	description.text = relic.description
	amount_gold_label.text = str(relic.price)

func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and not event.is_pressed():
			card_pressed.emit(relic)
