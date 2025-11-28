class_name TowerButton extends Control

signal pressed(tower_scene: PackedScene)

@export var tower_scene: PackedScene
@export var icon: Texture2D:
	set(value):
		icon = value
		_update_texture()

@export var icon_hover: AtlasTexture:
	set(value):
		icon_hover = value
		_update_texture_hover()

@export_multiline var tower_description: String:
	set(value):
		tower_description = value
		if hint_label:
			hint_label.text = value

var price: int = 0:
	set(value):
		price = value
		gold_price.price = value

@onready var tower_button: TextureButton = $VBoxContainer/CenterContainer/TowerButton
@onready var gold_price: GoldPrice = $VBoxContainer/GoldPrice
@onready var hint_container: PanelContainer = $HintContainer
@onready var hint_label: Label = $HintContainer/HintLabel


func _ready() -> void:
	_update_texture()
	hint_container.visible = false
	if hint_label:
		hint_label.text = tower_description
	
func _update_texture():
	if tower_button:
		tower_button.texture_normal = icon

func _update_texture_hover():
	if tower_button:
		tower_button.texture_hover = icon_hover

func _on_tower_button_pressed() -> void:
	if Score.gold < price:
		return
	pressed.emit(tower_scene)

func _on_tower_button_mouse_entered() -> void:
	hint_container.visible = true


func _on_tower_button_mouse_exited() -> void:
	hint_container.visible = false
