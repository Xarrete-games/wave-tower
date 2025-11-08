class_name ScoreUi extends Control

@onready var amount_gold_label: Label = $MarginContainer/VBoxContainer/AmountGoldLabel

func _ready():
	_on_gold_change(Score.gold)
	Score.gold_change.connect(_on_gold_change)
	
func _on_gold_change(amount: int) -> void:
	amount_gold_label.text = str(amount)
