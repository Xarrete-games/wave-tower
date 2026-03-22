class_name ItemOffer extends RefCounted

var item_data: ItemData

var price: int
var health_price: int


func _init(
	p_item_data: ItemData, 
	p_price: int, 
	p_health_price: int = 0) -> void:

	item_data = p_item_data
	price = p_price
	health_price = p_health_price

func create_item() -> Variant:
	if item_data is RelicData:
		return (item_data as RelicData).create_item()
	elif item_data is ConsumableData:
		return (item_data as ConsumableData).create_item()
	return null
