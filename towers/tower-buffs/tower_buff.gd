class_name TowerBuff extends RefCounted

var source: Source
var data
# duration
var duration: Duration
# if this buff is removed or expired, apply the residual buff
var residual_buff: TowerBuff

func _init(
	p_source: Source, 
	p_duration: Duration = null,
	p_residual_buff: TowerBuff = null,
	p_data = null) -> void:
	source = p_source
	duration = p_duration
	residual_buff = p_residual_buff
	data = p_data
