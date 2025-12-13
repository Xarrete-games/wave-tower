class_name Economy extends RefCounted

signal relics_discount_changed(relics_discount_mult: float)
signal extra_gold_dropped_change(amount: int)
signal available_free_towers_change(amount: int)
signal gold_change(amount: int)

var gold: int = 150:
	set(value):
		if value >= gold:
			AudioManager.play_coins()

		gold = value
		gold_change.emit(value)

var relics_discount_mult: float = 0.0:
	set(value):
		relics_discount_mult = value
		relics_discount_changed.emit(value)

var extra_gold_dropped: int = 0:
	set(value):
		extra_gold_dropped = value
		extra_gold_dropped_change.emit(extra_gold_dropped)

var available_free_towers: int = 0:
	set(value):
		available_free_towers = value
		available_free_towers_change.emit(available_free_towers)
	