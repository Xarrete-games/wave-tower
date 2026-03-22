class_name ChooseRelicCard extends Control

signal card_pressed(item_offer: ItemOffer)

const LABEL_SETTINGS_24_INVALID = preload("uid://c0seek6x1jue3")
const LABEL_SETTINGS_24 = preload("uid://bqa8xh2lpphdf")

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD

var item_offer: ItemOffer
var price: int = 0

var _has_enough_live = false
var _it_cost_health = false

@onready var description: RichTextLabel = $VBoxContainer/Description
@onready var title: Label = $VBoxContainer/Title
@onready var relic_texture: TextureRect = $RelicIcon/RelicTexture
@onready var hexagon_border: Polygon2D = $RelicIcon/Hexagon/Control/Root2d/HexagonBorder
@onready var health_price: HealthPrice = $VBoxContainer/HealthPrice

func _ready() -> void:
	health_price.visible = false

func set_relic(new_relic_value: ItemOffer) -> void:
	item_offer = new_relic_value
	var data: ItemData = new_relic_value.item_data
	relic_texture.texture = data.icon
	title.text = data.display_name
	description.text = data.description
	
	price = item_offer.price

	if item_offer.health_price > 0:
		health_price.visible = true
		_it_cost_health = true
		health_price.price = item_offer.health_price
		_chek_health(RunContext.status.health, item_offer.health_price)
		RunContext.status.health_change.connect(func (current_health: int) -> void:
			_chek_health(current_health, item_offer.health_price)
		)

	if data is RelicData:
		var relic_data: RelicData = data as RelicData
		hexagon_border.color =  RunContext.relics_manager.get_rarity_color(relic_data.rarity)
	
func _on_gui_input(event: InputEvent) -> void:
	if (_it_cost_health and not _has_enough_live):
		return
	
	if UIUtils.is_left_click_event(event):
		AudioManager.play_button_click()
		card_pressed.emit(item_offer)

func _chek_health(curren_health: int, health_cost: int) -> void:
	_has_enough_live = curren_health > health_cost
	
func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	relic_texture.custom_minimum_size = Vector2(130, 130)

func _on_mouse_exited() -> void:
	relic_texture.custom_minimum_size = Vector2(80, 80)
