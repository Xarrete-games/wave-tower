# score.gd
extends Node

signal gold_change(amount: int)
signal lives_change(amount: int)

var gold: int = 100:
	set(value):
		gold = value
		gold_change.emit(value)

var lives: int = 10:
	set(value):
		lives = value
		lives_change.emit(lives)
		
func add_gold(amount: int) -> void:
	gold += amount
	AudioManager.play_coins()
	gold_change.emit(gold)
	
func substract_gold(amount: int) -> void:
	gold -= amount
	gold_change.emit(gold)
