class_name DebuffBuilder

static func create_frost(source: Source) -> FrostDebuff:
	var data = DataLoader.get_debuff_data(EnemyDebuff.Type.FROST)
	return FrostDebuff.new(data, source)

static func create_burn(source: Source) -> BurnDebuff:
	var data = DataLoader.get_debuff_data(EnemyDebuff.Type.BURN)
	return BurnDebuff.new(data, source)

static func create_from_type(type: EnemyDebuff.Type, source: Source) -> EnemyDebuff:
	match type:
		EnemyDebuff.Type.FROST:
			return create_frost(source)
		EnemyDebuff.Type.BURN:
			return create_burn(source)
	push_error("[DebuffBuilder] Unknown debuff type: " + str(type))
	return null
