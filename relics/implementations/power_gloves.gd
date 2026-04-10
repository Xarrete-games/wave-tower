class_name PowerGloves extends Relic

const BUFF_ID: String = "damage_flat_buff"
const BUFF_VALUE: int = 3
const COUTER_THRESHOLD: int = 3
const TARGET_TOWER := 1

func on_consumable_used(consumable) -> void:
	if consumable is ConsumableTargeteable:
		var target_targeteable: ConsumableTargeteable = consumable as ConsumableTargeteable

		if target_targeteable.data.targeting_type == TARGET_TOWER:
			var tower: Tower = target_targeteable.target as Tower
			tower.add_buff(TowerBuffFactory.create_from_id(BUFF_ID, get_source(), BUFF_VALUE))
