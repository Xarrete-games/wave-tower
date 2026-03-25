class_name PriceContext
extends RefCounted

enum PriceType { TOWER, RELIC, CONSUMABLE }

var price_type: PriceType
var base_price: int
var discount: float
var final_price: int

func _init(p_price_type: PriceType, p_base_price: int):
	price_type = p_price_type
	base_price = p_base_price
	discount = 0.0
	final_price = base_price