class_name CaffeinePotion extends ConsumableUsable

var good_buff_id = id + "_good"
var bad_buff_id = id + "_bad"

func use() -> void:
	var debuff = TowerBuff.new(TowerBuff.SourceType.CONSUMABLE, bad_buff_id, AttackSpeedMultModifier.new(-0.2), 5)
	var buff = TowerBuff.new(TowerBuff.SourceType.CONSUMABLE, good_buff_id, AttackSpeedMultModifier.new(0.2), 5)
	buff.residual_buff = debuff
	RunContext.towers_buffs.add_buff(buff)
