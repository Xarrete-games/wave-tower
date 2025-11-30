class_name Rewardard extends Control

signal card_pressed(relic: Relic)
const LABEL_SETTINGS_24_INVALID = preload("uid://c0seek6x1jue3")
const LABEL_SETTINGS_24 = preload("uid://bqa8xh2lpphdf")

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD
var relic: Relic
var price: int = 0
var relic_colors: Dictionary[Relic.RelicRarity, Color] = {
	Relic.RelicRarity.COMMON: COMMON_COLOR,
	Relic.RelicRarity.RARE: RARE_COLOR,
	Relic.RelicRarity.EPIC: EPIC_COLOR,
}
var _has_enough_gold = false
var _has_enough_live = false
var _is_boniato = false

@onready var description: RichTextLabel = $VBoxContainer/Description
@onready var title: Label = $Title
@onready var amount_gold_label: Label = $VBoxContainer/Gold/AmountGoldLabel
@onready var relic_texture: TextureRect = $RelicTexture
@onready var hexagon_border: Polygon2D = $Control/HexagonBorder
@onready var amount_live_label: Label = $VBoxContainer/LivePriceContainer/AmountLiveLabel
@onready var live_price_container: HBoxContainer = $VBoxContainer/LivePriceContainer

func _ready() -> void:
	Score.gold_change.connect(_check_price)
	live_price_container.visible = false

func set_relic(new_relic_value: Relic) -> void:
	relic = new_relic_value
	relic_texture.texture = relic.texture
	title.text = relic.id
	description.text = relic.description
	amount_gold_label.text = str(relic.price)
	hexagon_border.color =  relic_colors[relic.rarity]
	price = relic.price
	_check_price(Score.gold)
	if relic is Boniato:
		live_price_container.visible = true
		_is_boniato = true
		_chek_live(LiveManager.lives)
		LiveManager.lives_change.connect(_chek_live)
	
func _on_gui_input(event: InputEvent) -> void:
	if not _has_enough_gold or (_is_boniato and not _has_enough_live):
		return

	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and not event.is_pressed():
			card_pressed.emit(relic)

func _chek_live(curren_live: int) -> void:
	_has_enough_live = curren_live > 5
	amount_live_label.label_settings = (
		LABEL_SETTINGS_24 
		if _has_enough_live
		else LABEL_SETTINGS_24_INVALID
	)

func _check_price(current_gold: int) -> void:
	_has_enough_gold = current_gold >= price
	amount_gold_label.label_settings = (
		LABEL_SETTINGS_24 
		if _has_enough_gold
		else LABEL_SETTINGS_24_INVALID
	)
	
func _on_mouse_entered() -> void:
	relic_texture.scale = Vector2(0.6, 0.6)

func _on_mouse_exited() -> void:
	relic_texture.scale = Vector2(0.5, 0.5)
