class_name ValveAmplifier extends Relic

const BUFF_ID: String = "attack_range_mult_buff"
const BUFF_VALUE: int = 10

func on_consumable_used(consumable: Consumable) -> void:
	if consumable is ConsumableTargeteable:
		var target_targeteable: ConsumableTargeteable = consumable as ConsumableTargeteable

		if target_targeteable.data.targeting_type == ConsumableTargeteable.TargetType.TOWER:
			var tower: Tower = target_targeteable.target as Tower
			tower.add_buff(TowerBuffFactory.create_from_id(BUFF_ID, get_source(), BUFF_VALUE))
