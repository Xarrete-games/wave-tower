class_name Economy extends RefCounted

signal relics_discount_changed(relics_discount_mult: float)
signal towers_discount_changed(towers_discount_mult: float)
signal consumable_discount_changed(consumables_discount_mult: float)
signal extra_gold_dropped_change(amount: int)
signal available_free_towers_change(amount: int)
signal gold_change(amount: int)

var is_soya_sauce_active: bool = false:
	set(value):
		is_soya_sauce_active = value
		relics_discount_mult = _relics_discount_mult
		towers_discount_mult = _towers_discount_mult
		consumables_discount_mult = _consumables_discount_mult

var gold: int = 1000:
	set(value):
		if value >= gold:
			AudioManager.play_coins()

		gold = value
		gold_change.emit(value)

var relics_discount_mult: float = 0.0:
	get:
		return _relics_discount_mult
	set(value):
		_relics_discount_mult = value * 2 if is_soya_sauce_active else value
		relics_discount_changed.emit(value)

var towers_discount_mult: float = 0.0:
	get:
		return _towers_discount_mult
	set(value):
		_towers_discount_mult = value * 2 if is_soya_sauce_active else value
		towers_discount_changed.emit(value)

var consumables_discount_mult: float = 0.0:
	get:
		return _consumables_discount_mult
	set(value):
		_consumables_discount_mult = value * 2 if is_soya_sauce_active else value
		consumable_discount_changed.emit(value)

var extra_gold_dropped: int = 0:
	set(value):
		extra_gold_dropped = value
		extra_gold_dropped_change.emit(extra_gold_dropped)

var available_free_towers: int = 0:
	set(value):
		available_free_towers = value
		available_free_towers_change.emit(available_free_towers)

	
var _relics_discount_mult: float = 0.0
var _towers_discount_mult: float = 0.0
var _consumables_discount_mult: float = 0.0