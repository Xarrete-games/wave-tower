class_name Rewardard extends Control

signal card_pressed(relic: Relic)
const LABEL_SETTINGS_24_INVALID = preload("uid://c0seek6x1jue3")
const LABEL_SETTINGS_24 = preload("uid://bqa8xh2lpphdf")

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD

var relic: Relic
var price: int = 0

var _has_enough_live = false
var _is_boniato = false

@onready var description: RichTextLabel = $VBoxContainer/Description
@onready var title: Label = $Title
@onready var relic_texture: TextureRect = $RelicTexture
@onready var hexagon_border: Polygon2D = $Control/HexagonBorder
@onready var amount_live_label: Label = $VBoxContainer/LivePriceContainer/AmountLiveLabel
@onready var live_price_container: HBoxContainer = $VBoxContainer/LivePriceContainer

func _ready() -> void:
	live_price_container.visible = false

func set_relic(new_relic_value: Relic) -> void:
	relic = new_relic_value
	relic_texture.texture = relic.texture
	title.text = relic.id
	description.text = relic.description
	hexagon_border.color =  RelicsManager.get_rarity_color(relic.rarity)
	price = relic.price

	if relic is Boniato:
		live_price_container.visible = true
		_is_boniato = true
		_chek_live(LiveManager.lives)
		LiveManager.lives_change.connect(_chek_live)
	
func _on_gui_input(event: InputEvent) -> void:
	if (_is_boniato and not _has_enough_live):
		return
	
	if Utils.is_left_click_event(event):
		AudioManager.play_button_click()
		card_pressed.emit(relic)

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
