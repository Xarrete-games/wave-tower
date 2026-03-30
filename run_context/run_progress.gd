class_name RunProgress extends RefCounted

signal current_wave_changed(wave_num: int)
signal current_wave_finished()
signal last_wave_finished()

var current_wave: int = 0:
    set(value):
        current_wave = value
        Hooks.on_wave_init()
        current_wave_changed.emit(current_wave)

var total_levels: int = 0
var total_waves: int = 0

func is_last_wave() -> bool:
    return current_wave >= total_waves