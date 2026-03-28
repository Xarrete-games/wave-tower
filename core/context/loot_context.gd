class_name LootContext extends RefCounted

var base_gold: int
var extra_gold: int
var gold_mult: int = 1
var chance_drop_consumable: int

func _init(extra_gold_p: int = 0, chance_drop_consumable_p: int = 50):
	extra_gold = extra_gold_p
	chance_drop_consumable = chance_drop_consumable_p

func get_total_gold() -> int:
	return (base_gold + extra_gold) * gold_mult