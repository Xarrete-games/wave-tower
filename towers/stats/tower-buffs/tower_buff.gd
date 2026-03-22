class_name TowerBuff extends RefCounted

enum Scope {
	GLOBAL,
	LOCAL
}

var source: Source
var modifier: TowerStatsModifier
var buff_data: BuffData
var scope: Scope
# duration
var duration: Duration
# if this buff is removed or expired, apply the residual buff
var residual_buff: TowerBuff

func _init(
	p_source: Source, 
	p_modifier: TowerStatsModifier, 
	p_duration: Duration = null,
	p_residual_buff: TowerBuff = null,
	p_buff_data: BuffData = null,
	p_scope: Scope = Scope.GLOBAL) -> void:
	source = p_source
	modifier = p_modifier
	duration = p_duration
	residual_buff = p_residual_buff
	buff_data = p_buff_data
	scope = p_scope
