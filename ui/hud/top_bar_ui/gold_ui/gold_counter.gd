class_name GoldCounter extends Control

@onready var amount_gold_label: Label = $AmountGoldLabel


var current_displayed_gold: float = 0.0
var target_gold: int = 0

const COUNTING_SPEED: float = 0.1 

func _ready():
	await RunContext.initialized
	target_gold = RunContext.economy.gold
	current_displayed_gold = float(RunContext.economy.gold)
	
	RunContext.economy.gold_change.connect(_on_gold_change)
	_update_label()


func _on_gold_change(amount: int) -> void:
	target_gold = amount


func _process(_delta: float) -> void:
	# update displayed value
	current_displayed_gold = lerp(current_displayed_gold, float(target_gold), COUNTING_SPEED)
	_update_label()
	
	# set value if it is very close
	if abs(current_displayed_gold - target_gold) < 0.01:
		current_displayed_gold = float(target_gold)

# Función auxiliar para actualizar el texto
func _update_label() -> void:
	amount_gold_label.text = str(int(round(current_displayed_gold)))
