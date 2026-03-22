class_name CaffeinePotion extends ConsumableUsable

var good_buff_id: String:
	get: return data.id + "_good"
var bad_buff_id: String:
	get: return data.id + "_bad"

func use() -> void:
	var duration = Duration.new(5, 0)
	var debuff = TowerBuff.new(Source.new(Source.SourceType.CONSUMABLE, bad_buff_id), AttackSpeedMultModifier.new(-0.2),duration)
	var buff = TowerBuff.new(Source.new(Source.SourceType.CONSUMABLE, good_buff_id), AttackSpeedMultModifier.new(0.2), duration)
	buff.residual_buff = debuff
	RunContext.towers_buffs.add_buff(buff)
