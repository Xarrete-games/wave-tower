class_name TowerButton extends Control

signal tower_button_pressed(tower_configuration: TowerDataWithInstance, price: int)
signal hover(tower_button: TowerButton)
signal unhover(tower_button: TowerButton)

const NORMAL_PANEL = preload("uid://dcjn1y7ofuii7")
const HOVER_PANEL = preload("uid://5m3jkdualcb3")

@export var tower_data: TowerDataWithInstance:
	set(value):
		tower_data = value
		configuration = tower_data.data
		tower_scene = tower_data.scene
		icon = configuration.icon
		type = configuration.type
		_set_new_price(configuration.base_price)
	
@export var panel: Panel	
@export var tower_button: TextureButton
@export var gold_price: GoldPrice
@export var amount_label: Label

var configuration: TowerData

var price: int = 0:
	set(value):
		price = value
		gold_price.price = value
var icon: Texture2D:
	set(value):
		icon = value
		_update_texture()

var icon_hover: AtlasTexture:
	set(value):
		icon_hover = value
		_update_texture_hover()

var amount: int = 0:
	set(value):
		amount = value
		amount_label.text = "x " + str(value)

var tower_scene: PackedScene
var type: Tower.Type

func _ready() -> void:
	RunContext.economy.available_free_towers_change.connect(_on_available_free_towers_change)
	RunContext.economy.towers_discount_changed.connect(_on_economy_towers_discount_changed)
	add_theme_stylebox_override("panel", NORMAL_PANEL)
	_update_texture()
	
func _update_texture():
	if tower_button:
		tower_button.texture_normal = icon

func _update_texture_hover():
	if tower_button:
		tower_button.texture_hover = icon_hover

func _on_mouse_exited() -> void:
	unhover.emit(self )
	panel.add_theme_stylebox_override("panel", NORMAL_PANEL)

func _on_mouse_entered() -> void:
	panel.add_theme_stylebox_override("panel", HOVER_PANEL)
	hover.emit(self)
	AudioManager.play_button_hover()

func _on_available_free_towers_change(available_free_towers: int) -> void:
	if available_free_towers > 0:
		price = 0
	else:
		if configuration:
			_set_new_price(configuration.base_price)

func _on_economy_towers_discount_changed(_towers_discount_mult: float) -> void:
	if RunContext.economy.available_free_towers > 0:
		price = 0
	else:
		if configuration:
			_set_new_price(configuration.base_price)

func _set_new_price(new_price: int) -> void:
	price = int(new_price * (1.0 - RunContext.economy.towers_discount_mult))

func _on_tower_button_pressed() -> void:
	AudioManager.play_button_click()
	if RunContext.economy.gold < price:
		return
	tower_button_pressed.emit(tower_data, price)
