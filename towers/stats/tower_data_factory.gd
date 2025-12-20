class_name TowerDataFactory

static func get_tower_buff(tower_type: Tower.Type) -> TowerBuff:
	match tower_type:
		Tower.Type.RED:
			return RedTowerBuff.new()
		Tower.Type.GREEN:
			return GreenTowerBuff.new()
		Tower.Type.BLUE:
			return BlueTowerBuff.new()
		_:
			return TowerBuff.new()