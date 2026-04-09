@abstract
class_name EnemyDebuff extends EnemyEffect

enum Type { FROST, BURN }

var type: Type
var data
var source: Source
var value: float = 0.0
var duration: float = 0.0
var tick_interval: float = 0.0
var max_stacks: int = 99

func _init(p_data, p_source: Source) -> void:
	data = p_data
	source = p_source
	type = p_data.debuff_type
	value = p_data.value
	duration = p_data.duration
	tick_interval = p_data.tick_interval
	max_stacks = p_data.max_stacks

static func create_frost(p_source: Source) -> EnemyDebuff:
	var p_data = DataLoader.get_debuff_data(Type.FROST)
	return FrostDebuff.new(p_data, p_source)

static func create_burn(p_source: Source) -> EnemyDebuff:
	var p_data = DataLoader.get_debuff_data(Type.BURN)
	return BurnDebuff.new(p_data, p_source)

static func create_from_type(p_type: Type, p_source: Source) -> EnemyDebuff:
	match p_type:
		Type.FROST:
			return create_frost(p_source)
		Type.BURN:
			return create_burn(p_source)
	push_error("[EnemyDebuff] Unknown debuff type: " + str(p_type))
	return null

func on_apply(enemy: Enemy):
	pass

func on_tick(enemy: Enemy):
	pass

func on_expire(enemy: Enemy):
	pass
