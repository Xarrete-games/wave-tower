class_name ItemData extends Resource

enum Type {
    CONSUMABLE,
    RELIC,
}
@export_group("General")
@export_multiline var id: String
@export_multiline var description: String
@export var texture: Texture2D
@export_group("Images")
@export var icon_48: Texture2D
@export var icon_48_used: Texture2D
@export_group("Price")
@export var price: int = 50
@export var health_price: int = 0
@export var price_increased: bool = true
@export_group("Script")
@export var runtime_script: Script

var type: Type