class_name GoldPrice extends HBoxContainer

const LABEL_SETTINGS_24_INVALID = preload("uid://c0seek6x1jue3")
const LABEL_SETTINGS_24 = preload("uid://bqa8xh2lpphdf")

var _has_enough_gold: bool = false

@onready var price_label: Label = $PriceLabel

var price: int = 0:
	set(value):
		price = value
		price_label.text = str(value)
		_check_label_color(Score.gold)

func _ready() -> void:
	Score.gold_change.connect(_check_label_color)
	
func _check_label_color(gold: int) -> void:
	_has_enough_gold = gold >= price
	price_label.label_settings = (
		LABEL_SETTINGS_24 
		if _has_enough_gold
		else LABEL_SETTINGS_24_INVALID
	)
	
