class_name TowerBuff extends RefCounted

enum SourceType {
	RELIC,
	TOWER,
	CONSUMABLE
}

var source_type: SourceType
var source: TowerB
var modifier: TowerStatsModifier
# duration
var duration: TowerBuffDuration
# if this buff is removed or expired, apply the residual buff
var residual_buff: TowerBuff

func _init(
	p_source_type: SourceType, 
	p_source_id: String, 
	p_modifier: TowerStatsModifier, 
	p_duration: TowerBuffDuration = null,
	p_residual_buff: TowerBuff = null) -> void:
	source_type = p_source_type
	source_id = p_source_id
	modifier = p_modifier
	duration = p_duration
	residual_buff = p_residual_buff
