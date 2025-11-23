class_name Rewardard extends Control

signal card_pressed(relic: Relic)

const COMMON_COLOR = Color.GREEN_YELLOW
const RARE_COLOR = Color.DODGER_BLUE
const EPIC_COLOR = Color.GOLD
var relic: Relic

@onready var description: RichTextLabel = $VBoxContainer/Description
@onready var title: Label = $Title
@onready var amount_gold_label: Label = $VBoxContainer/Gold/AmountGoldLabel
@onready var relic_texture: TextureRect = $RelicTexture
@onready var hexagon_border: Polygon2D = $Control/HexagonBorder

func set_relic(new_relic_value: Relic) -> void:
	relic = new_relic_value
	relic_texture.texture = relic.texture
	title.text = relic.id
	description.text = relic.description
	amount_gold_label.text = str(relic.price)
	if relic.rarity == Relic.RelicRarity.COMMON:
		hexagon_border.color = COMMON_COLOR
	if relic.rarity == Relic.RelicRarity.RARE:
		hexagon_border.color = RARE_COLOR
	if relic.rarity == Relic.RelicRarity.EPIC:
		hexagon_border.color = EPIC_COLOR

func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and not event.is_pressed():
			card_pressed.emit(relic)
