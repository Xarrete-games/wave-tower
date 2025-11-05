# score.gd
extends Node

signal gold_change(amount: int)
@onready var win_coins: AudioStreamPlayer2D = $Win_Coins

var gold: int = 50

func add_gold(amount: int) -> void:
	gold += amount
	win_coins.play()
	gold_change.emit(gold)
	
func substract_gold(amount: int) -> void:
	gold -= amount
	gold_change.emit(gold)

func _ready():
	pass
	
