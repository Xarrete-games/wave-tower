class_name DamageNumbers extends Control

@export var time_to_vanish: float = 2

@onready var damage_label: Label = $DamageLabel

func _ready():
	var tween1 = create_tween()
	var random_number: int = randi_range(-100, 100)
	tween1.tween_property(damage_label, "position", Vector2(random_number, -30), time_to_vanish / 2)
	var tween2 = create_tween()
	tween2.tween_property(damage_label, "modulate:a", 0.0, time_to_vanish / 2)
	await tween2.finished
	queue_free()
	
func set_attack(attack: Attack) -> void:
	damage_label.text = str(int(round(attack.damage)))
	if attack.is_critical or attack.is_execution:
		var critical_settings = damage_label.label_settings.duplicate()
		critical_settings.font_color = Color.RED
		damage_label.label_settings = critical_settings
		var tween = create_tween()
		tween.tween_property(damage_label.label_settings, "font_size", 48, time_to_vanish)
