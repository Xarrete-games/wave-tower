class_name TowerBuffStatsModifier extends TowerBuff

var stats_modifier: TowerStatsModifier

func _init(
	p_source: Source,
	p_stats_modifier: TowerStatsModifier,
	p_duration: Duration = null,
	p_residual_buff: TowerBuff = null,
	p_data: BuffData = null,
	p_scope: Scope = Scope.GLOBAL,
) -> void:
	super(p_source, p_duration, p_residual_buff, p_data, p_scope)
	stats_modifier = p_stats_modifier

func contribute(acc: TowerStatsAccumulator) -> void:
	if stats_modifier == null:
		return
	stats_modifier.contribute(acc)
