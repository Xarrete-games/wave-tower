class_name DebuffSlot extends Control

@export var texture_rect: TextureRect
@export var label: Label

var amount: int:
	set(value):
		amount = value
		if label:
			label.text = str(value)
			
var texture: Texture2D:
	set(value):
		texture = value
		texture_rect.texture = value
