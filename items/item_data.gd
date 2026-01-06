class_name ItemData extends Resource

enum Type {
    CONSUMABLE,
    RELIC,
}
@export_group("General")
@export_multiline var id: String
@export_multiline var description: String
@export_group("Images")
@export var texture: Texture2D
@export var icon_48: Texture2D
@export var icon_48_used: Texture2D
@export_group("Stats")
@export var rarity: Relic.Rarity
@export var max_stack: int = 1
@export var price: int = 50
@export var health_price: int = 0
@export var price_increased: bool = true
@export var type: Type
@export_group("Script")
@export var runtime_script: Script
