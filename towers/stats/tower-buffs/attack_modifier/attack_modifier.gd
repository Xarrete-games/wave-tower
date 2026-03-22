@abstract
class_name  AttackModifier extends RefCounted

enum SourceType {
    RELIC,
    TOWER,
    CONSUMABLE_TEMPORAL
}

var source_type: SourceType
var source_id: String

func _init(p_source_type: SourceType, p_source_id: String) -> void:
    source_type = p_source_type
    source_id = p_source_id

@abstract
func on_before_hit(ctx: AttackContext) -> void
