class_name GameItem extends RefCounted

enum Rarity { COMMON, RARE, EPIC}

var amount = 1
var id: String
var description: String
var rarity: Rarity
var max_stack: int
var texture: Texture2D
var base_price: int
var price: int
var price_increased: bool

func _init(data: ItemData) -> void:
	id = data.id
	description = data.description
	rarity = data.rarity
	max_stack = data.max_stack
	texture = data.texture
	base_price = data.price
	price = data.price
	price_increased = data.price_increased