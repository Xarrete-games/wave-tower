class_name PaganiniBow extends TowerBuffRelic

const BUFF_ID: String = "damage_mult_buff"
const BUFF_VALUE: int = 10

func _add_buff(tower: Tower) -> void:
	var tower_buff: TowerBuff = TowerBuffFactory.create_from_id(BUFF_ID, get_source(), BUFF_VALUE)
	if tower_buff != null:
		tower.add_buff(tower_buff)

func _remove_buff(tower: Tower) -> void:
	tower.remove_buff(get_source().type_id)

func _is_valid_tower(_tower: Tower) -> bool:
	return true
