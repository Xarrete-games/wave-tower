class_name Rewardard extends Control

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
@onready var title: Label = $Title
@onready var relic_texture: TextureRect = $RelicTexture
@onready var hexagon_border: Polygon2D = $Control/HexagonBorder
@onready var amount_live_label: Label = $VBoxContainer/LivePriceContainer/AmountLiveLabel
@onready var live_price_container: HBoxContainer = $VBoxContainer/LivePriceContainer

func _ready() -> void:
	live_price_container.visible = false

func set_relic(new_relic_value: ItemOffer) -> void:
	item_offer = new_relic_value
	var data: ItemData = new_relic_value.item_data
	relic_texture.texture = data.texture
	title.text = data.id
	description.text = data.description
	hexagon_border.color =  RelicsManager.get_rarity_color(data.rarity)
	price = item_offer.price

	if item_offer.health_price > 0:
		live_price_container.visible = true
		_it_cost_health = true
		_chek_live(LiveManager.lives)
		LiveManager.lives_change.connect(_chek_live)
	
func _on_gui_input(event: InputEvent) -> void:
	if (_it_cost_health and not _has_enough_live):
		return
	
	if Utils.is_left_click_event(event):
		AudioManager.play_button_click()
		card_pressed.emit(item_offer)

func _chek_live(curren_live: int) -> void:
	_has_enough_live = curren_live > 5
	amount_live_label.label_settings = (
		LABEL_SETTINGS_24 
		if _has_enough_live
		else LABEL_SETTINGS_24_INVALID
	)

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
	relic_texture.scale = Vector2(0.6, 0.6)

func _on_mouse_exited() -> void:
	relic_texture.scale = Vector2(0.5, 0.5)
