class_name ChooseRelicCard extends Control

signal card_pressed(relic_data)

const LABEL_SETTINGS_24_INVALID = preload("uid://c0seek6x1jue3")
const LABEL_SETTINGS_24 = preload("uid://bqa8xh2lpphdf")

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD

var relic_data

var _has_enough_live = false
var _it_cost_health = false

@onready var description: RichTextLabel = $VBoxContainer/Description
@onready var title: Label = $VBoxContainer/Title
@onready var relic_texture: TextureRect = $RelicIcon/RelicTexture
@onready var hexagon_border: Polygon2D = $RelicIcon/Hexagon/Control/Root2d/HexagonBorder
@onready var health_price: HealthPrice = $VBoxContainer/HealthPrice

func _ready() -> void:
	health_price.visible = false

func set_relic(new_relic_data) -> void:
	relic_data = new_relic_data
	var data = new_relic_data
	relic_texture.texture = data.icon
	title.text = data.display_name
	description.text = data.description

	if relic_data.health_price > 0:
		health_price.visible = true
		_it_cost_health = true
		health_price.price = relic_data.health_price
		_chek_health(RunContext.status.health, relic_data.health_price)
		RunContext.status.health_change.connect(func (current_health: int) -> void:
			_chek_health(current_health, relic_data.health_price)
		)

	hexagon_border.color =  RunContext.relics_manager.get_rarity_color(relic_data.rarity)
	
func _on_gui_input(event: InputEvent) -> void:
	if (_it_cost_health and not _has_enough_live):
		return
	
	if UIUtils.is_left_click_event(event):
		AudioManager.play_button_click()
		card_pressed.emit(relic_data)

func _chek_health(curren_health: int, health_cost: int) -> void:
	_has_enough_live = curren_health > health_cost
	
func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	relic_texture.custom_minimum_size = Vector2(130, 130)

func _on_mouse_exited() -> void:
	relic_texture.custom_minimum_size = Vector2(80, 80)
