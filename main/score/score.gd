# score.gd
extends Node

signal gold_change(amount: int)

var gold: int = 100:
	set(value):
		if value >= gold:
			AudioManager.play_coins()

		gold = value
		gold_change.emit(value)
