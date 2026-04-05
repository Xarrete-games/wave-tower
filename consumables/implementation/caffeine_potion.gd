class_name CaffeinePotion extends ConsumableUsable

var good_buff_id: String:
	get: return data.id + "_good"
var bad_buff_id: String:
	get: return data.id + "_bad"

func use() -> void:
	var duration = Duration.new(5, 0)
	var debuff_source = Source.new(Source.SourceType.CONSUMABLE, bad_buff_id, self)
	var buff_source = Source.new(Source.SourceType.CONSUMABLE, good_buff_id, self)
	var debuff = TowerBuffFactory.create_from_id("caffeine_bad_buff", debuff_source)
	var buff = TowerBuffFactory.create_from_id("caffeine_good_buff", buff_source)
	if debuff == null or buff == null:
		return
	debuff.duration = duration
	buff.duration = duration
	buff.residual_buff = debuff
	RunContext.towers_buffs.add_buff(buff)
