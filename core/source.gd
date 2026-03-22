class_name Source extends RefCounted

enum SourceType {
	RELIC,
	TOWER,
	CONSUMABLE,
	DEBUFF
}

## Source category (relic, tower, consumable, debuff...)
var type: SourceType
## Subtype identifier within the category (e.g., relic name, tower type, debuff type)
var type_id: String
## Optional: specific entity instance (e.g., which exact tower). Empty by default.
var id: String

func _init(p_type: SourceType, p_type_id: String, p_id: String = "") -> void:
	type = p_type
	type_id = p_type_id
	id = p_id