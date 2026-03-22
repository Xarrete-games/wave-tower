class_name ItemOffer extends RefCounted

var item_data: BaseData

var price: int
var health_price: int


func _init(
	p_item_data: BaseData, 
	p_price: int, 
	p_health_price: int = 0) -> void:

	item_data = p_item_data
	price = p_price
	health_price = p_health_price
