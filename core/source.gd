class_name Source extends RefCounted

enum SourceType {
	RELIC,
	TOWER,
	CONSUMABLE,
	DEBUFF,
	GLOBAL
}

## Source category (relic, tower, consumable, debuff...)
var type: SourceType
## Subtype identifier within the category (e.g., relic name, tower type, debuff type)
var type_id: String
## Optional: specific entity instance (e.g., which exact tower). Empty by default.
var entity: Variant
var origin: Source = null

func _init(p_type: SourceType, p_type_id: String, p_entity: Variant = null, p_origin: Source = null) -> void:
	type = p_type
	type_id = p_type_id
	entity = p_entity
	origin = p_origin