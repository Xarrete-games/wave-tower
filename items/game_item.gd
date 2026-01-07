class_name GameItem extends RefCounted



var amount = 1
var id: String
var description: String
var texture: Texture2D
var base_price: int
var price: int
var price_increased: bool
var type: ItemData.Type

func _init(data: ItemData) -> void:
	id = data.id
	description = data.description
	texture = data.texture
	base_price = data.price
	price = data.price
	price_increased = data.price_increased