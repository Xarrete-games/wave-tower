class_name BuffScheduler extends RefCounted

signal buff_expired(buff: TowerBuff)
signal buff_applied(buff: TowerBuff)

var progress

func _init(progress_p) -> void:
	progress = progress_p

func schedule(buff: TowerBuff) -> void:
	if buff.duration.seconds_duration > 0:
		_schedule_in_seconds(buff)
	elif buff.duration.waves_duration > 0:
		_schedule_in_waves(buff)

func _schedule_in_seconds(buff: TowerBuff) -> void:
	await Engine.get_main_loop().create_timer(buff.duration.seconds_duration, false).timeout
	_remove_buff(buff)

func _schedule_in_waves(buff: TowerBuff) -> void:
	var target_wave = progress.current_wave + buff.duration.waves_duration

	while progress.current_wave < target_wave:
		await progress.current_wave_finished

	_remove_buff(buff)

func _remove_buff(buff: TowerBuff) -> void:
	buff_expired.emit(buff)
	if buff.residual_buff:
		_add_residual(buff.residual_buff)

func _add_residual(buff: TowerBuff) -> void:
	buff_applied.emit(buff)
	if buff.duration:
		schedule(buff)
