class_name TowerBuff extends RefCounted

enum SourceType {
    RELIC,
    TOWER,
    LEVEL_UP,
    TEMPORAL_WAVE,
    CONSUMABLE
}

var source_type: SourceType
var source_id: String
var modifier: TowerStatsModifier
var duration: float
var residual_buff: TowerBuff

func _init(
    p_source_type: SourceType, 
    p_source_id: String, 
    p_modifier: TowerStatsModifier, 
    p_duration: float = 0) -> void:
    source_type = p_source_type
    source_id = p_source_id
    modifier = p_modifier
    duration = p_duration