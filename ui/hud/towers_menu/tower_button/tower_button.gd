class_name TowerButton extends Control

signal tower_button_pressed(tower_scene: PackedScene)
signal hover(tower_button: TowerButton)
signal unhover(tower_button: TowerButton)

const NORMAL_PANEL = preload("uid://dcjn1y7ofuii7")
const HOVER_PANEL = preload("uid://5m3jkdualcb3")

@export var tower_configuration: TowerConfigurationWithInstance:
	set(value):
		tower_configuration = value
		configuration = tower_configuration.configuration
		tower_scene = tower_configuration.scene
		icon = configuration.icon
		type = configuration.type
		
@export var panel: Panel	
@export var tower_button: TextureButton
@export var gold_price: GoldPrice

var configuration: TowerConfiguration

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

var tower_scene: PackedScene
var type: Tower.Type

func _ready() -> void:
	await RunContext.initialized
	price = RunContext.towers_price.get_price(type)
	RunContext.towers_price.tower_price_change.connect(_on_tower_price_change)
	RunContext.economy.available_free_towers_change.connect(_on_available_free_towers_change)
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

func _on_tower_price_change(tower_type: Tower.Type, new_price: int) -> void:
	if RunContext.economy.available_free_towers > 0:
		return
	if tower_type == type:
		price = new_price

func _on_available_free_towers_change(available_free_towers: int) -> void:
	if available_free_towers > 0:
		price = 0
	else:
		price = RunContext.towers_price.get_price(type)


func _on_tower_button_pressed() -> void:
	AudioManager.play_button_click()
	if RunContext.economy.gold < price:
		return
	tower_button_pressed.emit(tower_scene)
