class_name PaganiniBow extends Relic

const BUFF_ID: String = "damage_mult_buff"
const BUFF_VALUE: int = 10
const COUTER_THRESHOLD: int = 4

func on_tower_placed(tower: Tower) -> void:
	counter += 1
	if counter >= COUTER_THRESHOLD:
		counter = 0
		var tower_buff: TowerBuff = TowerBuffFactory.create_from_id(BUFF_ID, get_source(), BUFF_VALUE)
		if tower_buff != null:
			tower.add_buff(tower_buff)
