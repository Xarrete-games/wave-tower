extends Control

@onready var texture: TextureRect = $MarginContainer/texture
@onready var amount: Label = $MarginContainer/amount

func get_texture():
	return texture

func get_amount():
	return amount
