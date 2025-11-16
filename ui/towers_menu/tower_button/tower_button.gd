class_name TowerButton extends Control

signal pressed(tower_scene: PackedScene)

@export var tower_scene: PackedScene
@export var icon: Texture2D:
	set(value):
		icon = value
		_update_texture()

var price: int = 0:
	set = set_price

@onready var tower_button: TextureButton = $TowerButton
@onready var price_label: Label = $HBoxContainer/PriceLabel

func set_price(new_value: int) -> void:
	price = new_value
	price_label.text = str(price)
	
func _ready() -> void:
	_update_texture()

func _update_texture():
	if tower_button:
		tower_button.texture_normal = icon


func _on_tower_button_pressed() -> void:
	if Score.gold < price:
		return
	pressed.emit(tower_scene)
