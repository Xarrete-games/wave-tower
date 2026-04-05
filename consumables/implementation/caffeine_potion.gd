class_name CaffeinePotion extends ConsumableUsable

var good_buff_id: String:
	get: return data.id + "_good"
var bad_buff_id: String:
	get: return data.id + "_bad"

func use() -> void:
	var duration = Duration.new(5, 0)
	var debuff_modifier := TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_SPEED, TowerStatsModifier.Mode.MULT, -0.2)
	var buff_modifier := TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_SPEED, TowerStatsModifier.Mode.MULT, 0.2)
	var debuff = TowerBuffStatsModifier.new(Source.new(Source.SourceType.CONSUMABLE, bad_buff_id, self), debuff_modifier, duration)
	var buff = TowerBuffStatsModifier.new(Source.new(Source.SourceType.CONSUMABLE, good_buff_id, self), buff_modifier, duration)
	buff.residual_buff = debuff
	RunContext.towers_buffs.add_buff(buff)
