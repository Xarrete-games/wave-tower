@abstract
class_name TowerBuffStatsModifier extends TowerBuff

var value: float = 0.0

func _init(
	p_source: Source,
	p_duration: Duration = null,
	p_residual_buff: TowerBuff = null,
	p_data: BuffData = null,
	p_value: float = 0.0,
) -> void:
	super(p_source, p_duration, p_residual_buff, p_data)
	value = p_value

@abstract
func contribute(acc: TowerStatsAccumulator) -> void
 
