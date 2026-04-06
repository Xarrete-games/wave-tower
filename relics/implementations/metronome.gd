class_name Metronome extends Relic

const BUFF_ID: String = "attack_speed_mult_buff"
const BUFF_VALUE: int = 5

func on_wave_finished() -> void:
	var towers = RunContext.towers_manager.towers
	if towers.size() == 0:
		return
	
	towers.pick_random().add_buff(TowerBuffFactory.create_from_id(BUFF_ID, get_source(), BUFF_VALUE))