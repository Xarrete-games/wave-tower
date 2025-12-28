
class_name HealthPrice extends Control

const LABEL_SETTINGS_24_INVALID = preload("uid://c0seek6x1jue3")
const LABEL_SETTINGS_24 = preload("uid://bqa8xh2lpphdf")

var _has_enough_health: bool = false
var price: int = 0:
	set(value):
		price = value
		price_label.text = str(value)
		_check_label_color(RunContext.status.health)

@onready var price_label: Label = $PriceLabel

func _ready() -> void:
	RunContext.status.health_change.connect(_check_label_color)

func _check_label_color(health: int) -> void:
	_has_enough_health = health > price
	price_label.label_settings = (
		LABEL_SETTINGS_24 
		if _has_enough_health
		else LABEL_SETTINGS_24_INVALID
	)