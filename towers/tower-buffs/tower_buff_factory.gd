class_name TowerBuffFactory extends RefCounted

static func create_from_id(buff_id: String, source: Source, value: int) -> TowerBuff:
	var buff_data = DataLoader.get_tower_buff_data_by_id(buff_id)
	if buff_data == null:
		push_error("[TowerBuffFactory] No buff data found for id: %s" % buff_id)
		return null
	var buff = buff_data.create_item(source, value)
	if buff == null:
		push_error("[TowerBuffFactory] Could not create buff from data id: %s" % buff_id)
		return null
	return buff
