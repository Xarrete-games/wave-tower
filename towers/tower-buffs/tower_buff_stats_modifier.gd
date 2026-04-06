@abstract
class_name TowerBuffStatsModifier extends TowerBuff

func _init(
	p_source: Source,
	p_duration: Duration = null,
	p_residual_buff: TowerBuff = null,
	p_data: BuffData = null,
	p_scope: Scope = Scope.GLOBAL,
) -> void:
	super(p_source, p_duration, p_residual_buff, p_data, p_scope)

@abstract
func contribute(acc: TowerStatsAccumulator) -> void
 