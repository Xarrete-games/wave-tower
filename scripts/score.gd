# score.gd
extends Node

signal gold_change(amount: int)

var _gold: int = 50


func add_gold(amount: int) -> void:
	_gold += amount
	gold_change.emit(_gold)
	
func substract_gold(amount: int) -> void:
	_gold -= amount
	gold_change.emit(_gold)

func _ready():
	pass
	
