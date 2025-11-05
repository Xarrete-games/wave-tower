# score.gd
extends Node

signal gold_change(amount: int)

var gold: int = 50

func add_gold(amount: int) -> void:
	gold += amount
	AudioManager.play_coins()
	gold_change.emit(gold)
	
func substract_gold(amount: int) -> void:
	gold -= amount
	gold_change.emit(gold)


	
	
	
