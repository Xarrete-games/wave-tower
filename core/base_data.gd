class_name BaseData extends Resource

enum Rarity { COMMON, RARE, EPIC }

@export var id: String
@export var display_name: String
@export_multiline var description: String
@export var icon: Texture2D
@export var rarity: Rarity = Rarity.COMMON

func create_item() -> Variant:
	return null
