class_name Economy extends RefCounted

signal extra_gold_dropped_change(amount: int)
signal available_free_towers_change(amount: int)
signal gold_change(amount: int)


var is_sell_active: bool = false

var gold: int = 10000:
	set(value):
		if value >= gold:
			AudioManager.play_coins()

		gold = value
		gold_change.emit(value)

var extra_gold_dropped: int = 0:
	set(value):
		extra_gold_dropped = value
		extra_gold_dropped_change.emit(extra_gold_dropped)

var available_free_towers: int = 0:
	set(value):
		available_free_towers = value
		available_free_towers_change.emit(available_free_towers)

func add_gold(amount: int) -> void:
	gold += amount

func spend_gold(amount: int) -> bool:
	if gold >= amount:
		gold -= amount
		return true
	push_error("Not enough gold to spend: %d requested, %d available." % [amount, gold])
	return false
