class_name ItemOffer extends RefCounted

var item_data: ItemData

var price: int
var health_price: int
var price_increases: bool = true


func _init(
	p_relic_data: ItemData, 
	p_price: int, 
	p_price_increases: bool, 
	p_health_price: int) -> void:

	item_data = p_relic_data
	price = p_price
	price_increases = p_price_increases
	health_price = p_health_price

func create_item() -> Variant:
	return item_data.runtime_script.new(item_data)
