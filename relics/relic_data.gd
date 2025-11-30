class_name RelicData extends Resource

@export var id: String
@export_multiline var description: String
@export var texture: Texture2D
@export var type: Relic.RelicType
@export var rarity: Relic.RelicRarity
@export var max_stack: int = 10
@export var price: int = 50
@export var price_increased: bool = true
