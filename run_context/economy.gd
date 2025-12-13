class_name Economy extends RefCounted

signal relics_discount_changed(relics_discount_mult: float)

var relics_discount_mult: float = 0.0:
	set(value):
		relics_discount_mult = value
		relics_discount_changed.emit(value)
