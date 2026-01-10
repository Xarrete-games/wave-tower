@abstract
class_name DamageTakenModifier extends RefCounted

enum SourceType {
    RELIC,
    TOWER,
    CONSUMABLE_TEMPORAL
}

var source_type: SourceType
var source_id: String
var value: float

func _init(p_source_type: SourceType, p_source_id: String, p_value: float = 0.0) -> void:
	source_type = p_source_type
	source_id = p_source_id
	value = p_value

@abstract
func modify_damage(enemy: Enemy, acc: DamageTakenModifierAcc) -> void
