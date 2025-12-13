# score.gd
extends Node

signal gold_change(amount: int)
signal extra_gold_dropped_change(amount: int)

var gold: int = 100:
	set(value):
		if value >= gold:
			AudioManager.play_coins()

		gold = value
		gold_change.emit(value)

var extra_gold_dropped: int = 0:
	set(value):
		extra_gold_dropped = value
		extra_gold_dropped_change.emit(extra_gold_dropped)
