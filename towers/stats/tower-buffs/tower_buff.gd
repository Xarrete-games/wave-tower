class_name TowerBuff extends RefCounted

enum SourceType {
    RELIC,
    TOWER,
    LEVEL_UP,
    CONSUMABLE_TEMPORAL
}

var source_type: SourceType
var source_id: String
var modifier: TowerBuffModifier

func _init(p_source_type: SourceType = SourceType.LEVEL_UP, p_source_id: String = "", p_modifier: TowerBuffModifier = null) -> void:
    source_type = p_source_type
    source_id = p_source_id
    modifier = p_modifier