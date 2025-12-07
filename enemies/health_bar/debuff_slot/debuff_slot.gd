class_name DebuffSlot extends Control

@onready var label: Label = $MarginContainer/Label
@onready var texture_rect: TextureRect = $DebuffSlot

var amount: int:
	set(value):
		amount = value
		if label:
			label.text = str(value)
			
var texture: Texture2D:
	set(value):
		texture = value
		texture_rect.texture = value
