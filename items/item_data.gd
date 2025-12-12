class_name ItemData extends Resource

@export var id: String
@export_multiline var description: String
@export var texture: Texture2D
@export var rarity: Relic.Rarity
@export var max_stack: int = 10
@export var price: int = 50
@export var health_price: int = 0
@export var price_increased: bool = true
@export var runtime_script: Script
