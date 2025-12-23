class_name CristalLight extends Light2D

@export var attack_animation_time: float = 0.5
@export var max_scale: float = 2
@export var min_scale: float = 0.01
@export var max_energy: float = 2.0
@export var min_energy: float = 0

var _attack_scale_tween: Tween
var _attack_energy_tween: Tween

func play() -> void:
	await turn_on()
	turn_off()
	
func turn_on() -> void:
	_attack_energy_tween = create_tween()
	_attack_scale_tween = create_tween()
	_attack_energy_tween.tween_property(self, "energy", max_energy, attack_animation_time)
	_attack_scale_tween.tween_property(self, "texture_scale", max_scale, attack_animation_time)
	await _attack_energy_tween.finished

func turn_off() -> void:
	_attack_energy_tween = create_tween()
	_attack_scale_tween = create_tween()
	_attack_energy_tween.tween_property(self, "energy", min_energy, attack_animation_time)
	_attack_scale_tween.tween_property(self, "texture_scale", min_scale, attack_animation_time)
	await _attack_energy_tween.finished
	
func _kill_previous_animation() -> void:
	if _attack_scale_tween and _attack_scale_tween.is_running():
		_attack_scale_tween.kill()
	if _attack_energy_tween and _attack_energy_tween.is_running():
		_attack_energy_tween.kill()
