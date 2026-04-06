class_name DamageFlatBuff extends TowerBuffStatsModifier

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

static func create_instance(p_data: BuffData, p_source: Source, p_value: float) -> TowerBuff:
	var value: float = p_value
	return DamageFlatBuff.new(p_source, null, null, p_data, value)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_damage += value
